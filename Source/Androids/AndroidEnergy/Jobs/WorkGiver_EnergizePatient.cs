using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
	public class WorkGiver_RechargePatient : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest {
			get {
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		public override PathEndMode PathEndMode {
			get {
				return PathEndMode.ClosestTouch;
			}
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn patient = t as Pawn;
            Need_Energy patientNeed = patient?.needs?.TryGetNeed<Need_Energy>();

			if(patient == null || patient == pawn || patientNeed == null) {
				return false;
			}
			if(this.def.feedHumanlikesOnly && !patient.RaceProps.Humanlike) {
				return false;
			}
			if(this.def.feedAnimalsOnly && !patient.RaceProps.Animal) {
				return false;
			}
			if(patientNeed.EnergyNeed < EnergyNeedCategory.Moderate) {
				return false;
			}
			if(!RechargePatientUtility.ShouldBeManuallyRecharged(patient)) {
				return false;
			}
			LocalTargetInfo target = t;
			if(!pawn.CanReserve(target, 1, -1, null, forced)) {
				return false;
			}
			if(!EnergyUtility.TryFindBestEnergyRechargeSource(pawn, patient, patientNeed.EnergyNeed >= EnergyNeedCategory.Critical, out Thing rechargeSource)) {
				JobFailReason.Is("AT.Job.FailReason.NoEnergySources".Translate());
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn patient = (Pawn)t;
			Need_Energy patientNeed = patient.needs.TryGetNeed<Need_Energy>();
			if(EnergyUtility.TryFindBestEnergyRechargeSource(pawn, patient, patientNeed.EnergyNeed >= EnergyNeedCategory.Critical, out Thing rechargeSource)) {
				return new Job(JobDefOf.FeedPatient, rechargeSource, patient) {
					count = EnergyConsumableUtility.WillIngestStackCountOf(patient, rechargeSource.def)
				};
			}
			return null;
		}
	}
}
