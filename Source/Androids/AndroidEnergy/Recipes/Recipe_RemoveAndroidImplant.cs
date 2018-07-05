using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Verse;
using RimWorld;
using Harmony;

namespace MOARANDROIDS
{
    //based on Recipe_RemoveBodyPart but that is internal so ...
    public class Recipe_RemoveAndroidImplant : Recipe_Surgery
    {
        private const float ViolationGoodwillImpact = 20f;

        [DebuggerHidden]
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            IEnumerable<BodyPartRecord> parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);
            foreach (BodyPartRecord part in parts)
            {
				if(pawn.health.hediffSet.hediffs.Any(hediff => hediff.Part == part
											&& hediff.TryGetComp<HediffComp_AndroidImplant>() != null))
					yield return part;
            }
        }

        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
			return pawn.Faction != billDoerFaction;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff existingImplant = pawn.health.hediffSet.hediffs.FirstOrDefault(hediff => hediff.Part == part
                                                                && hediff.TryGetComp<HediffComp_AndroidImplant>() != null);
                                                                
            if(existingImplant == null) {
                Log.Error($"Attempted to remove implant in part {part.def.LabelCap} on {pawn.Name}, but could not find a valid installed implant");
                return;
            }
            
            if(billDoer != null && base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill)) 
                    return;
            
            pawn.health.RemoveHediff(existingImplant);

			if(part.def.tags.Contains("NeedsImplantablePart"))
				pawn.health.AddHediff(EnergyHediffs.AT_NotPresent, part);
			
			if(billDoer != null){		
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });

				var removedImplantThingDef = existingImplant.def.GetImplantThingDef();
				if(removedImplantThingDef != null) {
					Thing removedImplantThing = ThingMaker.MakeThing(removedImplantThingDef);
					removedImplantThing.TryGetComp<ThingComp_AndroidImplant>()?.LoadStateFromHediffComp(
					existingImplant.TryGetComp<HediffComp_AndroidImplant>());
					GenDrop.TryDropSpawn(removedImplantThing, billDoer.Position, billDoer.Map, ThingPlaceMode.Near,
						out Thing oldDroppedImplant, placedAction: null);
				}
				else {
					Log.Message($"Could not find ImplantThingDef for ImplantHediff {existingImplant.def.defName}");
				}
            }
           
            if (this.IsViolationOnPawn(pawn, part, Faction.OfPlayer))
            {
                pawn.Faction.AffectGoodwillWith(billDoer.Faction, -20f);
            }
        }

        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
        {
			string implantLabel = pawn.health.hediffSet.hediffs
				.FirstOrDefault(hediff => hediff.Part == part
						&& hediff.TryGetComp<HediffComp_AndroidImplant>() != null)
				?.Label ?? "ERROR";
			return "AT_RemoveImplantRecipe.SurgeryLabel".Translate(implantLabel);
        }
    }
}
