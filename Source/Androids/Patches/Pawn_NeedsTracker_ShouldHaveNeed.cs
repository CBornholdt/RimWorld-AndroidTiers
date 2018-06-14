using System;
using Verse;
using Harmony;
using RimWorld;

namespace MOARANDROIDS
{
    [HarmonyPatch(typeof(RimWorld.Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    static class Pawn_NeedsTracker_ShouldHaveNeed
    {
		static void Postfix(Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result)
		{
			Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_NeedsTracker), "pawn").GetValue(__instance);
			bool flag = (nd != AndroidNeeds.AT_Energy || pawn.TryGetComp<CompNeedsEnergy>() != null);
			__result = __result && flag;
		}
    }
}
