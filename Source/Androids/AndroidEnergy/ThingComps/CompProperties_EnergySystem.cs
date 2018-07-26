using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;

namespace MOARANDROIDS
{
    public class CompProperties_EnergySystem : CompProperties
    {
		public float baseEnergyConsumptionPer1000Ticks = 0.1f;
		public HediffDef initialReactor;
		public HediffDef initialConduit;
		public HediffDef initialBattery;
		public List<ThingDef> initialComponentTypes;

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach(var error in base.ConfigErrors(parentDef))
				yield return error;

			var invalidInitialComponents = initialComponentTypes?
					.Where(thingDef => !thingDef.comps.Any(prop => prop.compClass.IsSubclassOf(typeof(EnergySystemComp))));
			foreach(var badThingDef in invalidInitialComponents ?? Enumerable.Empty<ThingDef>())
				yield return string.Format("Error with initial energy system components, {0} is not valid EnergySystemComp Thing", badThingDef.label);
                
            if(parentDef.comps.Count(compProps => compProps.compClass == typeof(EnergySystem)) > 1)
                yield return string.Format("Error, multiple energy systems detected in {0}", parentDef.label);
		}

		public CompProperties_EnergySystem() => this.compClass = typeof(EnergySystem);

		public override void ResolveReferences(ThingDef parentDef)
		{
			parentDef.recipes.AddRange(
				DefDatabase<RecipeDef>.AllDefsListForReading
					.Where(recipeDef => recipeDef.HasModExtension<DefModExt_AutoIncludeWithEnergySystems>()
								&& !parentDef.recipes.Contains(recipeDef)));

			Traverse.Create(parentDef).Field("allRecipesCached").SetValue(null);
		}
	}
}
