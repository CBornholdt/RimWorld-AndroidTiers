using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;
using Harmony;

namespace MOARANDROIDS
{
    public class JobDriver_StartMindUpload : JobDriver
    {   
		public float workPerformed = 0;
    
		public TargetIndex CasketIndex => TargetIndex.A;
		public TargetIndex ImplantIndex => TargetIndex.B;

		public LocalTargetInfo Casket => TargetA;
		public LocalTargetInfo Implant => TargetB;

		public override bool TryMakePreToilReservations()
		{
			return pawn.Reserve(this.TargetA, this.job, maxPawns: 1, stackCount: -1, layer: null)
					&& pawn.Reserve(this.TargetB, this.job, maxPawns: 1, stackCount: 1, layer: null);
		}

		public float TotalWorkNeeded => 1000f;
		public float WorkPerTick => (float) this.pawn.skills.GetSkill(SkillDefOf.Intellectual).Level;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.workPerformed, "WorkPerformed", 0);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(CasketIndex).FailOnForbidden(CasketIndex);

			Toil gotoImplant = Toils_Goto.GotoThing(ImplantIndex, PathEndMode.ClosestTouch);
			gotoImplant.AddPreTickAction(() => {
				if(!this.TargetA.Thing.Spawned) {
					var potentialImplants = WorkGiver_StartMindUpload.AvailableBlankImplants(this.pawn);
					if(!potentialImplants.Any()) {
						this.EndJobWith(JobCondition.Incompletable);
						Messages.Message("AT.Job.StartMindUpload.NoMoreBlankImplants".Translate(), Casket.Thing, MessageTypeDefOf.NegativeEvent);
					}
					this.job.SetTarget(ImplantIndex, potentialImplants.MinBy(implant =>
						 implant.PositionHeld.DistanceToSquared(Casket.Cell)));
					gotoImplant.initAction();   //Repath to item
				}
			});

			yield return gotoImplant;
			yield return Toils_Haul.StartCarryThing(ImplantIndex);
			yield return Toils_Goto.GotoThing(CasketIndex, PathEndMode.InteractionCell);

			Toil startUploadToil = new Toil() {
				initAction = () => Implant.Thing.Destroy(DestroyMode.Vanish),
				tickAction = () => {
					workPerformed += WorkPerTick;
					if(workPerformed >= TotalWorkNeeded) {
						(Casket.Thing as Building_MindUploadCasket).BeginMindUploadProcess();
						this.ReadyForNextToil();
					}
				},
	            defaultCompleteMode = ToilCompleteMode.Never
			};
			startUploadToil.WithProgressBar(CasketIndex, () => 1f - workPerformed / TotalWorkNeeded);
			yield return startUploadToil;
		}
	}
}
