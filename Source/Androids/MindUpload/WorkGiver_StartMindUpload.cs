using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class WorkGiver_StartMindUpload : WorkGiver_Scanner
    {
        static public IEnumerable<Thing> AvailableBlankImplants(Pawn pawn) => pawn.Map.listerThings
                    .ThingsMatching(ThingRequest.ForDef(AndroidImplants.AT_MindCaptureImplantEmpty))
                    .Where(thing => pawn.CanReserveAndReach(thing, PathEndMode.ClosestTouch,
                        maxDanger: Danger.Deadly));
    
        //TODO split for performance if ever automated
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Building_MindUploadCasket casket = t as Building_MindUploadCasket;

			if(casket == null)
				return null;
                        
			if(!AvailableBlankImplants(pawn).Any()) {
				JobFailReason.Is("AT.Work.StartMindUpload.NoImplantsAvailable".Translate());
				return null;
			}

			if(!casket.IsReadyForMindUpload()) {
				JobFailReason.Is("AT.Work.StartMindUpload.CasketNotReady".Translate());
				return null;
			}

			var chosenBlankImplant = AvailableBlankImplants(pawn).MinBy(implant =>
				implant.PositionHeld.DistanceToSquared(t.InteractionCell));

			return new Job(MiscJobs.AT_StartMindUpload, t, chosenBlankImplant);
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_MindUploadCasket>()
                .Where(casket => casket.TryGetComp<CompPowerTrader>()?.PowerOn ?? false)
                .Cast<Thing>();                                         
		}
	}
}
