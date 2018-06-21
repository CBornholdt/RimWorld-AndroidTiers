using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;

namespace MOARANDROIDS
{
    public class JobGiver_RechargeEnergy : ThinkNode_JobGiver
    {
		override public float GetPriority(Pawn pawn)
		{
			var energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
			if(energyNeed == null)
				return 0;

			//Log.Message("here for " + pawn + " " + energyNeed.CurLevel + " " + energyNeed.LowEnergyNeed);

			switch(energyNeed.EnergyNeed) {
				case EnergyNeedCategory.Minor:
					return 1;
                case EnergyNeedCategory.Moderate:
                    return 5f;
				case EnergyNeedCategory.Major:
					return 8f;
				case EnergyNeedCategory.Critical:
					return 10;
                default:
					return 0f;
			}        
		}

		override protected Job TryGiveJob(Pawn pawn)
		{
			var energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
			if((energyNeed?.EnergyNeed ?? EnergyNeedCategory.None) == EnergyNeedCategory.None)
				return null;

			var energySystem = pawn.TryGetComp<EnergySystem>();
			if(energySystem == null)
				return null;

            
			IEnumerable<CompPowerBattery_EnergyAdapterComp> potentialBatteriesToRechargeAt = pawn.Map
								.AllBuildingsWithComp<CompPowerBattery_EnergyAdapterComp>()
								.Where(battery => pawn.CanReserveAndReach(battery, PathEndMode.Touch, pawn.NormalMaxDanger()
														, maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false))
								.Select(battery => battery.TryGetComp<CompPowerBattery_EnergyAdapterComp>())
								.Where(bComp => bComp.StoredEnergyPct > EnergyConstants.BatteryRechargeNormalThresh    //Full drainage shield
												&& !energySystem.AttachedSources.Contains(bComp));
			
            if(!potentialBatteriesToRechargeAt.Any())
				return null;

			var chosenRechargeSource = potentialBatteriesToRechargeAt
											.MinBy(bComp => (pawn.Position - bComp.Position()).LengthHorizontalSquared
																	* bComp.StoredEnergyPct).parent;

			var whenToDisconnect = DisconnectWhenTag.NotTouching | DisconnectWhenTag.EnergySystemFull
							| DisconnectWhenTag.SourceSlow | DisconnectWhenTag.SourceLow;

			return new EnergySystemJob() {
				def = EnergyJobs.AT_ConnectSourceThenDisconnect,
				disconnectWhen = new DisconnectWhen(whenToDisconnect),
				targetA = chosenRechargeSource
			};                 
                
		/*	float minPowerToSeekBattery = 0.05f * energyNeed.Props.powerForFullEnergy;
			var potentialBatteriesToRechargeAt = pawn.Map.AllBuildingsWithComp<CompPowerBattery>()
					.Where(battery => pawn.CanReserveAndReach(battery, PathEndMode.ClosestTouch, pawn.NormalMaxDanger()
											, maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false)
										&& battery.TryGetComp<CompPowerBattery>().StoredEnergy >= minPowerToSeekBattery);

			if(!potentialBatteriesToRechargeAt.Any())
				return null;

			Building chosenRechargeBuilding = potentialBatteriesToRechargeAt.MinBy
									(building => (pawn.Position - building.Position).LengthHorizontalSquared);

			return new Job(EnergyJobs.AT_RechargeEnergy, chosenRechargeBuilding);   */
		}
    }
}
