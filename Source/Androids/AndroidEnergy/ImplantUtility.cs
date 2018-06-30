using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;


namespace MOARANDROIDS
{
    static public class ImplantUtility
    {
		static public ThingDef GetImplantThingDef(this HediffDef implantedHediff) =>
			DefDatabase<RecipeDef>.AllDefsListForReading
									.FirstOrDefault(recipeDef => recipeDef.addsHediff == implantedHediff)
									?.fixedIngredientFilter.AllowedThingDefs
									.FirstOrDefault(thingDef => thingDef.IsAndroidImplant());

		static public HediffDef GetImplantedHediffDef(this ThingDef implant) =>
			DefDatabase<RecipeDef>.AllDefsListForReading
									.FirstOrDefault(recipeDef => recipeDef.fixedIngredientFilter.Allows(implant))
									?.addsHediff;
                                            
		static public bool IsAndroidImplant(this ThingDef thingDef) =>
			thingDef.comps.Any(prop => typeof(ThingComp_AndroidImplant).IsAssignableFrom(prop.compClass));                                 
    }
}
