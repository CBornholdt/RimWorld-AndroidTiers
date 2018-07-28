using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;
using UnityEngine;

namespace MOARANDROIDS
{
	public class EnergyAdapter_PowerBattery : CompPowerBattery, IEnergySource, IEnergySystemConnectable
    {
        private List<Tuple<Pawn, int>> connectedPawns = new List<Tuple<Pawn, int>>();
		private int ticker;
		private int numCurrentlyConnected = 0;

		public bool WasRecentlyConnected(Pawn pawn) => connectedPawns.Any(sp => sp.Item1 == pawn);

		public void MarkConnected(Pawn pawn) => connectedPawns.Add(Tuple.Create(pawn, Find.TickManager.TicksGame));
    
		public new EnergyAdapter_PowerBatteryProps Props =>
				(EnergyAdapter_PowerBatteryProps)this.props;

		public SourceStatusType SourceStatus => (this.StoredEnergy > 0)
						? SourceStatusType.Active : SourceStatusType.Disabled;

		public float SourcePriority => Props.sourcePriority;

		public float DesiredSourceRatePer1000Ticks => Props.desiredTranferRatePer1000Ticks;

		public float CurrentMaxSourcableEnergy => base.StoredEnergy / Props.powerPerUnitEnergy;

		public ThingWithComps Parent => this.parent;

		public string ForcedWorkFloatMenuOptionText => "AT.Job.ConnectBattery.OptionLabel".Translate();

		public int SimultaneousConnections => Props.maxCurrentConnections;

		public void SourceEnergy(float amount)
		{
			float powerLost = amount * Props.powerPerUnitEnergy;
			this.DrawPower(Mathf.Min(powerLost, base.StoredEnergy));
		}                         

		public override void PostExposeData()
		{
			base.PostExposeData();
			this.ForceRegisterReferenceable(); 
		}

		public string GetUniqueLoadID() => this.parent.GetUniqueLoadID() + "_EAC";

		public override void Initialize(CompProperties props)
		{
			ticker = Rand.Range(0, 250);
			base.Initialize(props);
		}
     
		public override void CompTick()
		{
			if(--ticker <= 0) {
				ticker = 250;
				CompTickRare_bug();
			}
			base.CompTick();
		}

        //Had a bug with CompTickRare, using this idiom instead
		public void CompTickRare_bug()
		{
			int reconnectTicks = AT_Mod.settings.energySearchSettings.batteryReconnectionTicks;
			for(int i = connectedPawns.Count - 1; i >= 0; i--) {
				if(connectedPawns[i].Item2 + reconnectTicks >= Find.TickManager.TicksGame) {
					Log.Message($"Removing {connectedPawns[i].Item1.Name} from recently connected list on {this.parent.ToString()}");
					connectedPawns.RemoveAt(i);
				}
			}
			base.CompTickRare();
		}

		public void SourceAttached(EnergySystem system)
		{
			this.numCurrentlyConnected++;
		}


		public void SourceDetached(EnergySystem system)
		{
			this.numCurrentlyConnected--;
			MarkConnected(system.Owner);
		}

		public ConnectWhen WhenToConnect()
		{
			return new ConnectWhen(ConnectWhenTag.WhenTouching);
		}

		public DisconnectWhen WhenToDisconnect()
		{
			var tag = DisconnectWhenTag.NotTouching | DisconnectWhenTag.EnergySystemFull;

			if(AT_Mod.settings.energySearchSettings.allowCriticalToOverrideProtection)
				tag = tag | DisconnectWhenTag.SourceLow_StorageNotCritical;
			else
				tag = tag | DisconnectWhenTag.SourceLow;
                            
			return new DisconnectWhen(tag);
		}

		public bool IsAvailableFor(EnergySystem system, bool isCritical)
		{
			if(isCritical)
				return numCurrentlyConnected < Props.maxCurrentConnections;
			else
				return (numCurrentlyConnected < Props.maxCurrentConnections)
					&& StoredEnergyPct >= AT_Mod.settings.energySearchSettings.batteryProtectionLevel;
		}

		public bool IsAvailableFor(EnergySystem system, bool isCritical, out string unavailableReason)
		{
			unavailableReason = null;
			if(numCurrentlyConnected >= Props.maxCurrentConnections) {
				unavailableReason = "AT.Job.PowerBattery.UnavailableReason.TooManyConnections"
					.Translate(Props.maxCurrentConnections);
				return false;
			}
			if(!isCritical && StoredEnergyPct < AT_Mod.settings.energySearchSettings.batteryProtectionLevel) {
				unavailableReason = "AT.Job.PowerBattery.UnavailableReason.BatteryDrainProtection"
					.Translate();
				return false;
			}
			return true;    	
		}
	}
}
