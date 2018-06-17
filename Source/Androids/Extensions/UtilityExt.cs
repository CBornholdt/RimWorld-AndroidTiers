﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    static public class UtilityExt
    {
		static public IEnumerable<Building> AllBuildingsOfType_L<T>(this Map map) where T : Building
		{
			return map.listerThings.AllThings.OfType<T>().Cast<Building>();
		}  
        
        static public IEnumerable<Building> AllBuildingsOfType<T>(this Map map) where T : Building
        {
            return map.AllBuildingsOfType_L<T>();
        }
        
        static public IEnumerable<Building> AllBuildingsOfDef_L(this Map map, ThingDef buildingDef)
        {
			return map.listerThings.AllThings.OfType<Building>().Cast<Building>()
							.Where(building => building.def == buildingDef);
        }
        
        static public IEnumerable<Building> AllBuildingsOfDef(this Map map, ThingDef buildingDef)
        {
			return map.AllBuildingsOfDef_L(buildingDef);
        }
        
        static public IEnumerable<Building> AllBuildingsWithComp_L<T>(this Map map) where T : ThingComp
        {
            return map.listerThings.AllThings.OfType<Building>().Cast<Building>()
                            .Where(building => building.TryGetComp<T>() != null);
        }
        
        static public IEnumerable<Building> AllBuildingsWithComp<T>(this Map map) where T : ThingComp
        {
			return map.AllBuildingsWithComp_L<T>();
        }
    }
}