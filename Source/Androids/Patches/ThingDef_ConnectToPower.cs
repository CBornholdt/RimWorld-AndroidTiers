using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Harmony;
using UnityEngine;


namespace MOARANDROIDS
{  
    //Changed direct class comparison to IsAssignableFrom to allow for subclasses
    [HarmonyPatch(typeof(ThingDef))]
    [HarmonyPatch(nameof(ThingDef.ConnectToPower), Harmony.PropertyMethod.Getter)]
    static public class ThingDef_ConnectToPower
    {
		static bool Prefix(ThingDef __instance, ref bool __result)
		{
			if(__instance.EverTransmitsPower) {
				__result = false;
				return false;
			}
			for(int i = 0; i < __instance.comps.Count; i++) {
				if(typeof(CompPowerBattery).IsAssignableFrom(__instance.comps[i].compClass)) {
					__result = true;
					return false;
				}
				if(typeof(CompPowerTrader).IsAssignableFrom(__instance.comps[i].compClass)) {
					__result = true;
                    return false;
				}
			}
		
			return false;
		}
    }
}
