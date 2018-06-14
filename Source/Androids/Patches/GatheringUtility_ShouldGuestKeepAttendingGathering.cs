using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [HarmonyPatch(typeof(RimWorld.GatheringsUtility))]
    [HarmonyPatch(nameof(RimWorld.GatheringsUtility.ShouldGuestKeepAttendingGathering))]
    static public class GatheringUtility_ShouldGuestKeepAttendingGathering
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			MethodInfo restCurCategoryGetter = AccessTools.Property(typeof(Need_Rest)
                                , nameof(Need_Rest.CurCategory)).GetGetMethod();
            MethodInfo helper = AccessTools.Method(typeof(GatheringUtility_ShouldGuestKeepAttendingGathering)
                                            , nameof(GatheringUtility_ShouldGuestKeepAttendingGathering
                                                    .AddNullCheckToRestCurCategory));

            foreach(var code in instructions) 
                if(code.opcode == OpCodes.Callvirt && code.operand == restCurCategoryGetter) {
                    yield return new CodeInstruction(OpCodes.Call, helper); //Consume 1, return RestCategory
                }
                else
                    yield return code;
        }

        static public RestCategory AddNullCheckToRestCurCategory(Need_Rest rest)
        {
			return rest?.CurCategory ?? RestCategory.Rested;
        }
    }
}
 