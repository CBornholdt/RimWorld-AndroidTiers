using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    static public class UtilityExt
    {
		static public T TryGetCompInterface<T>(this ThingWithComps thingWithComps) where T : class
		{
            for(int i = 0; i < thingWithComps.AllComps.Count; i++)
                if(thingWithComps.AllComps[i] is T t)
                    return t;
            return null;
		}
    
		static public T TryGetCompInterface<T>(this Thing thing) where T : class
		{
			return (thing as ThingWithComps)?.TryGetCompInterface<T>();
		}
    
		static public IEnumerable<T> AllBuildingsOfType_L<T>(this Map map) where T : Building
		{
			return map.listerThings.AllThings.OfType<T>().Cast<T>();
		}  
        
        static public IEnumerable<T> AllBuildingsOfType<T>(this Map map) where T : Building
        {
            return map.AllBuildingsOfType_L<T>();
        }
        
        static public IEnumerable<Building> AllBuildingsOfDef(this Map map, ThingDef buildingDef)
        {
			return map.listerThings.AllThings.OfType<Building>().Cast<Building>()
							.Where(building => building.def == buildingDef);
        }
        
        static public IEnumerable<T> AllBuildingsWithComp<T>(this Map map) where T : class
        {
			return map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial)
							.Select(thing => thing.TryGetCompInterface<T>())
							.Where(tComp => tComp != null);
        }
    }
}
