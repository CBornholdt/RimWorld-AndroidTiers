using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class WorkGiver_Warden_DeliverEnergy : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Pawn prisoner = (Pawn)t;
            Need_Energy prisonerNeed = prisoner?.needs?.TryGetNeed<Need_Energy>();

            if(prisoner == null || pawn == prisoner || prisonerNeed == null)
                return null;
            
            if (!base.ShouldTakeCareOfPrisoner(pawn, t))
            {
                return null;
            }
            if (!prisoner.guest.CanBeBroughtFood)
            {
                return null;
            }
            if (!prisoner.Position.IsInPrisonCell(prisoner.Map))
            {
                return null;
            }
            if (prisonerNeed.EnergyNeed < EnergyNeedCategory.Moderate)
            {
                return null;
            }
            if (WardenFeedUtility.ShouldBeFed(prisoner))
            {
                return null;
            }
            if(!EnergyUtility.TryFindBestEnergyRechargeSource(pawn, prisoner, prisonerNeed.EnergyNeed >= EnergyNeedCategory.Critical, out Thing rechargeSource)) {
				return null;
            }
            
            if (rechargeSource.GetRoom(RegionType.Set_Passable) == prisoner.GetRoom(RegionType.Set_Passable))
            {
                return null;
            }
            if (WorkGiver_Warden_DeliverEnergy.EnergyAvailableInRoomTo(prisoner))
            {
                return null;
            }
            return new Job(JobDefOf.DeliverFood, rechargeSource, prisoner)
            {   //TODO adjust count here to reflect number of prisoners
                count = EnergyConsumableUtility.WillIngestStackCountOf(prisoner, rechargeSource.def),
                targetC = RCellFinder.SpotToChewStandingNear(prisoner, rechargeSource)
            };
        }

        private static bool EnergyAvailableInRoomTo(Pawn prisoner)
        {
            if (prisoner.carryTracker.CarriedThing != null && prisoner.carryTracker.CarriedThing.ConsumableEnergy() > 0f)
            {
                return true;
            }
            float energyInRoom = 0f;
            float dailyNeededEnergy = 0f;
            Room room = prisoner.GetRoom(RegionType.Set_Passable);
            if (room == null)
            {
                return false;
            }
            for (int i = 0; i < room.RegionCount; i++)
            {
                Region region = room.Regions[i];
                List<Thing> list = region.ListerThings.ThingsInGroup(ThingRequestGroup.HaulableAlways);
                for (int j = 0; j < list.Count; j++)
                {
                    Thing thing = list[j];
                    if (thing.IsEnergyConsumable())
						energyInRoom += thing.ConsumableEnergy();
					if(thing.IsEnergySource())
						energyInRoom += thing.GetMaxSourceableEnergy();
                }
                List<Thing> list2 = region.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                for (int k = 0; k < list2.Count; k++)
                {
                    Pawn pawn = list2[k] as Pawn;
                    if (pawn.IsPrisonerOfColony && (pawn.carryTracker.CarriedThing == null 
                                                        || prisoner.carryTracker.CarriedThing.ConsumableEnergy() <= 0))
						dailyNeededEnergy += EnergyUsageUtility.ExpectedUsagePerDay(pawn);
  
                }
            }
            return energyInRoom >= 0.9 * dailyNeededEnergy;
        }
    }
}
