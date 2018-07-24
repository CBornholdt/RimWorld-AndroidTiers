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
		float uploadingWork = 0f;
        
        private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
        private Material barFilledCachedMat;
        private static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);
        private static readonly Color BarCompletedColor = new Color(0.9f, 0.85f, 0.2f);
    
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

		public float TotalWorkNeeded => 100f;

		public float Progress => uploadingWork / TotalWorkNeeded;

		public override void Tick()
		{
			if(CurrentlyUploading) {
				uploadingWork += 1;
				if(uploadingWork >= TotalWorkNeeded)
					CompleteMindUpload();
			}
		}

		public bool IsReadyForMindUpload()
		{
			return innerContainer.Any(thing => (thing is Corpse corpse && corpse.InnerPawn.IsValidForMindUpload())
											|| (thing is Pawn pawn && pawn.IsValidForMindUpload()));
		}

		public void BeginMindUploadProcess()
		{
			currentlyUploading = true;
			uploadingWork = 0;
		}

		public void CompleteMindUpload()
		{


		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.currentlyUploading, "CurrentlyUploading", false);
			Scribe_Values.Look<float>(ref this.uploadingWork, "UploadingProgress", 0f);
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
        
        private Material BarFilledMat
        {
            get
            {
                if (this.barFilledCachedMat == null)
                {
                    this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp
                        (Building_MindUploadCasket.BarZeroProgressColor, Building_MindUploadCasket.BarCompletedColor
                                    , this.Progress), false);
                }
                return this.barFilledCachedMat;
            }
        }

		public override void Draw()
		{
			base.Draw();
			if(this.CurrentlyUploading) {
				Vector3 drawPos = this.DrawPos;
				drawPos.y += 0.046875f;
				drawPos.z += 0.25f;
				GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest() {
					center = drawPos,
					size = Building_MindUploadCasket.BarSize,
					fillPercent = (float)Progress,
					filledMat = this.BarFilledMat,
					unfilledMat = Building_MindUploadCasket.BarUnfilledMat,
					margin = 0.1f,
					rotation = Rot4.North
				});
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
