using System;
using System.Linq;
using System.Collections.Generic;
using Verse;

namespace MOARANDROIDS
{
    static public class EnergyThingExt
    {
        static public bool IsEnergyConsumable(this Thing thing) =>
            thing.TryGetComp<ThingComp_EnergyConsumable>() != null;
    
        static public float ConsumableEnergy(this Thing thing) =>
            EnergyConsumableUtility.GetConsumableEnergyFor(thing.def);
        
        //YAGNI STRIKES AGAIN
        static public float ConsumableEnergyWithin(this Thing thing)
        {
			float energy = EnergyConsumableUtility.GetConsumableEnergyFor(thing.def);
            
            void addInnerThingsRecur(IThingHolder holder)
			{
				var childHolders = new List<IThingHolder>();
				holder.GetChildHolders(childHolders);
				foreach(var childHolder in childHolders)
					addInnerThingsRecur(childHolder);
				foreach(var heldThing in holder.GetDirectlyHeldThings())
					energy += EnergyConsumableUtility.GetConsumableEnergyFor(heldThing.def);
			}

			if(thing is IThingHolder thingHolder)
				addInnerThingsRecur(thingHolder);

			return energy;
        }

		static public bool IsEnergySource(this Thing thing) =>
			(thing as ThingWithComps)?.AllComps.Any(comp => comp is IEnergySource) ?? false;

		static public IEnergySource GetEnergySourceOrNull(this Thing thing) =>
			(thing as ThingWithComps)?.AllComps.FirstOrDefault(comp => comp is IEnergySource)
				as IEnergySource;

		static public float GetMaxSourceableEnergy(this Thing thing) =>
			thing.GetEnergySourceOrNull()?.CurrentMaxSourcableEnergy ?? 0f;
            
        static public bool IsEnergySink(this Thing thing) =>
            (thing as ThingWithComps)?.AllComps.Any(comp => comp is IEnergySink) ?? false;
            
        static public IEnergySink GetEnergySinkOrNull(this Thing thing) =>
            (thing as ThingWithComps)?.AllComps.FirstOrDefault(comp => comp is IEnergySink)
                as IEnergySink;
                
        static public float GetMaxSinkableEnergy(this Thing thing) =>
            thing.GetEnergySinkOrNull()?.CurrentMaxSinkableEnergy ?? 0f;
    }
}
