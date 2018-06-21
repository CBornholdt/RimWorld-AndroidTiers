using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
    public class JobDriver_ConnectSourceThenDisconnect : JobDriver
    {
        public readonly TargetIndex SourceThingInd = TargetIndex.A;

        override public bool TryMakePreToilReservations()
        {
            return this.pawn.Reserve(TargetA, this.job, maxPawns: 1, stackCount: -1, layer: null);
        }

        override protected IEnumerable<Toil> MakeNewToils()
        {
            //Either Thing is a source or a component of thing is a source
            IEnergySource source = TargetA.Thing as IEnergySource;
            source = source ?? ((TargetA.Thing as ThingWithComps).AllComps
                    .FirstOrDefault(comp => comp is IEnergySource) as IEnergySource);
            EnergySystem energySystem = this.pawn.TryGetComp<EnergySystem>();
            EnergySystemJob esJob = (this.job as EnergySystemJob);
            
            if(source == null) {
                Log.Warning($"Attempted to start job {job.def.label} with target {TargetA.Thing.Label} but it is not a source");
                yield break;
            }
            if(energySystem == null) {
                Log.Warning($"Attempted to start job {job.def.label} with pawn {this.pawn.Name} but pawn did not have an energy system");
                yield break;
            }
            if(esJob == null) {
                Log.Warning($"Attempted to start job {job.def.label} with pawn {this.pawn.Name} but job was not an EnergySystemJob");
                yield break;
            }
        
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);

            Toil connectSource = new Toil();
            connectSource.initAction = () => energySystem.AttachSource(source);
			if(esJob.disconnectWhen.whenTag != DisconnectWhenTag.Never) {
				connectSource.defaultCompleteMode = ToilCompleteMode.Never;
				connectSource.tickAction = () => {
					if(esJob.disconnectWhen.ShouldDisconnectSystemFrom(energySystem, source))
						this.ReadyForNextToil();
				};
			}
                        
            yield return connectSource;

			if(esJob.disconnectWhen.whenTag != DisconnectWhenTag.Never) {
				Toil disconnectSource = new Toil();
				disconnectSource.initAction = () => energySystem.DetachSource(source);
				yield return disconnectSource;
			}
        }
    }
}
