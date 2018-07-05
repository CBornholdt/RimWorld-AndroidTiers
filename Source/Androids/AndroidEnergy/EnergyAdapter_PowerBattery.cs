using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;
using UnityEngine;

namespace MOARANDROIDS
{
	public class EnergyAdapter_PowerBattery : CompPowerBattery, IEnergySource
    {
		private List<Tuple<Pawn, int>> connectedPawns = new List<Tuple<Pawn, int>>();

		public bool WasRecentlyConnected(Pawn pawn) => connectedPawns.Any(sp => sp.Item1 == pawn);

		public void MarkConnected(Pawn pawn) => connectedPawns.Add(Tuple.Create(pawn, Find.TickManager.TicksGame));
    
		public new EnergyAdapter_PowerBatteryProps Props =>
				(EnergyAdapter_PowerBatteryProps)this.props;

		public SourceStatusType SourceStatus => (this.StoredEnergy > 0)
						? SourceStatusType.Active : SourceStatusType.Disabled;

		public float SourcePriority => Props.sourcePriority;

		public float DesiredSourceRatePer1000Ticks => Props.desiredTranferRatePer1000Ticks;

		public float CurrentMaxSourcableEnergy => base.StoredEnergy / Props.powerPerUnitEnergy;

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

		public override void CompTickRare()
		{
			int reconnectTicks = AT_Mod.settings.energySearchSettings.batteryReconnectionTicks;
			for(int i = connectedPawns.Count - 1; i >= 0; i--)
				if(connectedPawns[i].Item2 + reconnectTicks >= Find.TickManager.TicksGame)
					connectedPawns.RemoveAt(i);       
		}

		public void SourceAttached(EnergySystem system) { }
		
		public void SourceDetached(EnergySystem system) => MarkConnected(system.Owner);	
	}
}
