﻿using System;
using System.Linq;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
 
namespace MOARANDROIDS
{
    [HarmonyPatch(typeof(RimWorld.HealthCardUtility))]
    [HarmonyPatch("GetTooltip")]
    static public class HealthCardUtility_GetTooltip
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo tipStringExtraGetter = AccessTools.Property(typeof(Hediff), nameof(Hediff.TipStringExtra)).GetGetMethod();
            MethodInfo labelHelper = AccessTools.Method(typeof(HealthCardUtility_GetTooltip)
                                        , nameof(HealthCardUtility_GetTooltip.TransformBleedingToLeakingIfAndroid));

            foreach(var code in instructions) {
                yield return code;
                if(code.opcode == OpCodes.Callvirt && code.operand == tipStringExtraGetter) {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);  //string, Pawn on stack
                    yield return new CodeInstruction(OpCodes.Call, labelHelper); //Consume 2, leave string
                }
            }   
        }

        static public string TransformBleedingToLeakingIfAndroid(string original, Pawn pawn)
        {
            if(pawn.IsAndroid())
                return original.Replace("BleedingRate".Translate(), "AT_Leaking".Translate());
            return original;
        }
    }
}
