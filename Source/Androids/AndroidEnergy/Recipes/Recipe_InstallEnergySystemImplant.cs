using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Recipe_InstallEnergySystemImplant : Recipe_InstallImplantAndroid
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
			ThingComp_AndroidImplant implantComp = ingredients.Select(thing => thing.TryGetComp<ThingComp_AndroidImplant>())
                                                              .First(comp => comp != null);
			if(implantComp == null) {
				Log.Error($"Attempted to install implant for {this.recipe.defName} on {pawn.Name}, but could not find a valid implant in ingredients");
				return;
			}
            
            if(billDoer != null && base.CheckSurgeryFailAndroid(billDoer, pawn, ingredients, part, bill)) 
                    return;

			Hediff notPresentHediff = pawn.health.hediffSet.hediffs.FirstOrDefault(hediff => hediff.Part == part
																&& hediff.def == EnergyHediffs.AT_NotPresent);
			if(notPresentHediff != null)
				pawn.health.RemoveHediff(notPresentHediff);    

            Hediff existingHediff = pawn.health.hediffSet.hediffs.FirstOrDefault(hediff => hediff.Part == part
                                                                && hediff.TryGetComp<HediffComp_AndroidImplant>() != null);
			if(existingHediff != null) 
			    pawn.health.RemoveHediff(existingHediff);
                
            Hediff implantedHediff = HediffMaker.MakeHediff(recipe.addsHediff, pawn);
                implantedHediff.TryGetComp<HediffComp_AndroidImplant>()?.LoadStateFromThingComp(implantComp);
                pawn.health.AddHediff(implantedHediff, part);

			if(billDoer != null) {
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});

				var removedImplantThingDef = existingHediff?.def.GetImplantThingDef();
				if(existingHediff != null && removedImplantThingDef != null) {   
					Thing removedImplantThing = ThingMaker.MakeThing(removedImplantThingDef);
					removedImplantThing.TryGetComp<ThingComp_AndroidImplant>()?.LoadStateFromHediffComp(
						existingHediff.TryGetComp<HediffComp_AndroidImplant>());
					GenDrop.TryDropSpawn(removedImplantThing, pawn.Position, pawn.Map, ThingPlaceMode.Near,
						out Thing oldDroppedImplant, placedAction: null);
				}
			}	
        }
    }
}
