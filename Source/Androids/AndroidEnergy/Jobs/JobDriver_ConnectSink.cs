using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
    public class JobDriver_ConnectSink : JobDriver
    {
        public readonly TargetIndex SinkThingInd = TargetIndex.A;

        override public bool TryMakePreToilReservations()
        {
            int maxPawns = TargetA.Thing.TryGetCompInterface<IEnergySystemConnectable>()?.SimultaneousConnections
                ?? 1;
            return this.pawn.Reserve(TargetA, this.job, maxPawns: maxPawns, stackCount: -1, layer: null);
        }

        override protected IEnumerable<Toil> MakeNewToils()
        {
            //Either Thing is a sink or a component of thing is a sink
            IEnergySink sink = TargetA.Thing as IEnergySink;
            sink = sink ?? ((TargetA.Thing as ThingWithComps).AllComps
                    .FirstOrDefault(comp => comp is IEnergySink) as IEnergySink);
            EnergySystem energySystem = this.pawn.TryGetComp<EnergySystem>();
            EnergySystemJob esJob = (this.job as EnergySystemJob);
            
            if(sink == null) {
                Log.Warning($"Attempted to start job {job.def.label} with target {TargetA.Thing.Label} but it is not a sink");
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

            Toil connectSink = new Toil();
            connectSink.initAction = () => energySystem.AttachSink(sink, esJob.disconnectWhen);
            yield return connectSink;
        }
    }
}
