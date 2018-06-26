using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompProperties_EnergySystem : CompProperties
    {
		public float baseEnergyConsumptionPer1000Ticks = 0.1f;
		public List<ThingDef> initialComponentTypes;
		public int maxInstalledComps = 3;

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
	}
}
