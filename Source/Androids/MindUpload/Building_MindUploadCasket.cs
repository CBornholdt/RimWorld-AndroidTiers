using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

namespace MOARANDROIDS
{
    //Will copy code from Building_CryptosleepCasket but I don't want to actually be one ...
    public class Building_MindUploadCasket : Building_Casket
    {
		bool currentlyUploading = false;
    
		/*Should allow pawn to decay/if left inside too long ...
		public override void Tick()
		{
            base.Tick();
			if(innerContainer.Any())
				innerContainer[0].Tick();
            if (Find.TickManager.TicksGame % 250 == 0)
            {
                this.TickRare();
				if(innerContainer.Any())
					innerContainer[0].TickRare();
            }
		}       */

		public bool CurrentlyUploading => currentlyUploading;

		public bool IsReadyForMindUpload()
		{
			return innerContainer.Any(thing => (thing is Corpse corpse && corpse.InnerPawn.IsValidForMindUpload())
											|| (thing is Pawn pawn && pawn.IsValidForMindUpload()));
		}

		public void BeginMindUploadProcess()
		{
			currentlyUploading = true;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.currentlyUploading, "CurrentlyUploading", false);
		}

		//From Building_CryptosleepCasket
		public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    SoundDef.Named("CryptosleepCasketAccept").PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(selPawn))
            {
                yield return o;
            }
            if (this.innerContainer.Count == 0)
            {
				if(selPawn.IsRobot()) {
                    FloatMenuOption failer1 = new FloatMenuOption("AT.Job.NotAllowed.NotForRobots".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    yield return failer1;
				}
                else if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.ByPawn)) {
                    FloatMenuOption failer2 = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                    yield return failer2;
                }
                else
                {
					JobDef jobDef = MiscJobs.AT_EnterMindUploadCasket;
                    string jobStr = "AT.Job.EnterMindUploadCasket.OptionLabel".Translate();
                    Action jobAction = delegate
                    {
                        Job job = new Job(jobDef, this);
                        selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    };
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, jobAction, MenuOptionPriority.Default, null, null, 0f, null, null), selPawn, this, "ReservedBy");
                }
            }
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            if (base.Faction == Faction.OfPlayer && this.innerContainer.Count > 0 && this.def.building.isPlayerEjectable)
            {
                Command_Action eject = new Command_Action();
                eject.action = new Action(this.EjectContents);
                eject.defaultLabel = "CommandPodEject".Translate();
                eject.defaultDesc = "CommandPodEjectDesc".Translate();
                if (this.innerContainer.Count == 0)
                {
                    eject.Disable("CommandPodEjectFailEmpty".Translate());
                }
                eject.hotKey = KeyBindingDefOf.Misc1;
                eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);
                yield return eject;
            }
        }

        public override void EjectContents()
        {
			ThingDef filthSlime = RimWorld.ThingDefOf.FilthSlime;
            foreach (Thing current in ((IEnumerable<Thing>)this.innerContainer))
            {
                Pawn pawn = current as Pawn;
                if (pawn != null)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(filthSlime);
                }
            }
            if (!base.Destroyed)
            {
                SoundDef.Named("CryptosleepCasketEject").PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
            }
            base.EjectContents();
        }

		public static Building_MindUploadCasket FindMindUploadCasketFor(Thing pawnOrCorpse, Pawn traveler, bool ignoreOtherReservations)
		{
			Building_MindUploadCasket casket = (Building_MindUploadCasket)GenClosest.ClosestThingReachable
					(pawnOrCorpse.Position, pawnOrCorpse.Map, ThingRequest.ForDef(AndroidBuildings.AT_MindUploadCasket)
						, PathEndMode.InteractionCell, TraverseParms.For(traveler, Danger.Deadly, TraverseMode.ByPawn, canBash: false)
						, maxDistance: 9999f, validator: thing => !((Building_MindUploadCasket)thing).HasAnyContents
																	&& traveler.CanReserve(thing, maxPawns: 1)
						, customGlobalSearchSet: null, searchRegionsMin: 0, searchRegionsMax: -1, forceGlobalSearch: false
						, traversableRegionTypes: RegionType.Set_Passable, ignoreEntirelyForbiddenRegions: false);
			return casket;
		}
    }
}
