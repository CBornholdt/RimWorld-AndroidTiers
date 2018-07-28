using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class WorkGiver_Warden_Recharge : WorkGiver_Warden
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
            
            if (!WardenFeedUtility.ShouldBeFed(prisoner))
            {
                return null;
            }
            
            if(prisonerNeed.EnergyNeed < EnergyNeedCategory.Moderate) {
                return null;
            }
			if(EnergyUtility.TryFindBestEnergyRechargeSource(pawn, prisoner, prisonerNeed.EnergyNeed >= EnergyNeedCategory.Critical, out Thing rechargeSource)) {
				return new Job(JobDefOf.FeedPatient, rechargeSource, prisoner) {
					count = EnergyConsumableUtility.WillIngestStackCountOf(prisoner, rechargeSource.def)
				};
			}
			else {
				JobFailReason.Is("AT.Job.FailReason.NoEnergyConsumable".Translate());
				return null;
			}
        }
    }
}
