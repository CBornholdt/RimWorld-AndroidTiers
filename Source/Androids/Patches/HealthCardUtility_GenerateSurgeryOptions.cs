using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using Verse;
using Verse.AI;
using Harmony;
using UnityEngine;

namespace MOARANDROIDS
{
   // [HarmonyPatch("<GenerateSurgeryOption>c__AnonStorey4")]
   // [HarmonyPatch("<>m__0")]
    static public class HealthCardUtility_GenerateSurgeryOptions
    {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{		
			MethodInfo isFleshPropertyGetter = typeof(RaceProperties).GetProperty(nameof(RaceProperties.IsFlesh)).GetGetMethod();
			MethodInfo helperMethod = typeof(HealthCardUtility_GenerateSurgeryOptions).GetMethod("Helper");
            
			foreach(var instruction in instructions) {
				if(instruction.opcode == OpCodes.Callvirt && instruction.operand == isFleshPropertyGetter) {
					yield return new CodeInstruction(OpCodes.Call, helperMethod) { labels = instruction.labels };
				}
				else
					yield return instruction;
			}
		}

		public static bool Helper(RaceProperties race) => Traverse.Create(race).Field("fleshType").GetValue<FleshTypeDef>()
					?.requiresBedForSurgery ?? true;
    }
}
