using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
    public class JobDriver_InstallEnergySystemComp : JobDriver
    {
		int totalTicksNeeded;
		int ticksWorked;
        
        public readonly TargetIndex CompToInstall = TargetIndex.A;

		public JobDriver_InstallEnergySystemComp()
		{
			totalTicksNeeded = 200;
			ticksWorked = 0;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.totalTicksNeeded, "TotalTicksNeeded");
			Scribe_Values.Look<int>(ref this.ticksWorked, "TicksWorked");
		}

		override public bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.TargetA, this.job, maxPawns: 1, stackCount: -1, layer: null);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			EnergySystem energySystem = this.pawn.TryGetComp<EnergySystem>();
			EnergySystemComp comp = this.TargetA.Thing.TryGetComp<EnergySystemComp>();

			if(comp == null) {
				Log.Message($"Attempted to start job {job.def.label} with target {TargetA.Thing.Label} but it is not an EnergySystemComp");
				yield break;
			}

			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);

			Toil installToil = new Toil();
			installToil.tickAction = () => {
				if(++ticksWorked >= totalTicksNeeded)
					this.ReadyForNextToil();
			};
			installToil.defaultCompleteMode = ToilCompleteMode.Never;
			installToil.WithProgressBar(TargetIndex.A, () => (float)this.ticksWorked / (float)this.totalTicksNeeded
								, interpolateBetweenActorAndTarget: false, offsetZ: -0.5f);
			yield return installToil;                                

			yield return new Toil() {
				initAction = () => energySystem.InstallEnergySystemComp(comp)
			};
		}
	}
}
