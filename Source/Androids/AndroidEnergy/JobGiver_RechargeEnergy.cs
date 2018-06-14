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

			switch(energyNeed.LowEnergyNeed) {
				case EnergyNeedCategory.Acceptable:
					return 1;
				case EnergyNeedCategory.Low:
					return 8f;
				case EnergyNeedCategory.Critical:
					return 10;
                case EnergyNeedCategory.Empty:
					return 15f;
                default:
					return 1;
			}        
		}

		override protected Job TryGiveJob(Pawn pawn)
		{
			var energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
			if(energyNeed == null || energyNeed.CurLevel >= energyNeed.RechargeThreshold)
				return null;
                
			float minPowerToSeekBattery = 0.05f * energyNeed.Props.powerForFullEnergy;
			var potentialBatteriesToRechargeAt = pawn.Map.AllBuildingsWithComp<CompPowerBattery>()
					.Where(battery => pawn.CanReserveAndReach(battery, PathEndMode.ClosestTouch, pawn.NormalMaxDanger()
											, maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false)
										&& battery.TryGetComp<CompPowerBattery>().StoredEnergy >= minPowerToSeekBattery);

			if(!potentialBatteriesToRechargeAt.Any())
				return null;

			Building chosenRechargeBuilding = potentialBatteriesToRechargeAt.MinBy
									(building => (pawn.Position - building.Position).LengthHorizontalSquared);

			return new Job(EnergyJobs.AT_RechargeEnergy, chosenRechargeBuilding);
		}
    }
}
