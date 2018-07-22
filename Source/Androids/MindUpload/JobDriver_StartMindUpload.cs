using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class JobDriver_StartMindUpload : JobDriver
    {
		public TargetIndex CasketIndex => TargetIndex.A;
		public TargetIndex ImplantIndex => TargetIndex.B;

		public LocalTargetInfo Casket => TargetA;
		public LocalTargetInfo Implant => TargetB;

		public override bool TryMakePreToilReservations()
		{
			return pawn.Reserve(this.TargetA, this.job, maxPawns: 1, stackCount: -1, layer: null)
					&& pawn.Reserve(this.TargetB, this.job, maxPawns: 1, stackCount: 1, layer: null);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(CasketIndex).FailOnForbidden(CasketIndex);

			Toil gotoImplant = Toils_Goto.GotoThing(ImplantIndex, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(ImplantIndex)
                .FailOnSomeonePhysicallyInteracting(ImplantIndex);
			yield break;
		}
	}
}
