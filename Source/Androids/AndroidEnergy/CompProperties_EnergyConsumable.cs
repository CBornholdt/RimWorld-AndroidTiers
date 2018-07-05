using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MOARANDROIDS
{
    public class CompProperties_EnergyConsumable : CompProperties
    {
		public float energyAmount;
		public int ticksToConsume = 100;
    
        public CompProperties_EnergyConsumable()
        {
			this.compClass = typeof(ThingComp_EnergyConsumable);
        }

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			if(!parentDef.IsIngestible)
				yield return $"{parentDef.defName} requires an ingestible section if it is to be an EnergyConsumable";
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			parentDef.ingestible.foodType = parentDef.ingestible.foodType.AddEnergyConsumableFlag();
		}
	}
}
