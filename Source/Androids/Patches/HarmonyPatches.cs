using System.Reflection;
using System;
using Harmony;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace MOARANDROIDS
{
    [StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.rwmods.androidtiers");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //Need to patch annoying nested type, cannot use annotations
			Type nestedType = typeof(HealthCardUtility).GetNestedType("<GenerateSurgeryOption>c__AnonStorey4"
                            , BindingFlags.NonPublic | BindingFlags.Public);
			harmony.Patch(nestedType.GetMethod("<>m__0", BindingFlags.NonPublic | BindingFlags.Instance),
				null, null, new HarmonyMethod(typeof(HealthCardUtility_GenerateSurgeryOptions).GetMethod("Transpiler")));
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("NotifyPlayerOfKilled")]
    internal static class DeadPawnMessageRemoval
    {
        private static bool Prefix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff, Caravan caravan)
        {
            Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool flag = value.kindDef == MOARANDROIDS.PawnKindDefOf.MicroScyther;
            return !flag;
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("MakeDowned")]
    internal static class DiesUponDowned
    {
        private static bool Prefix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff)
        {
            Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if(value.kindDef == MOARANDROIDS.PawnKindDefOf.MicroScyther || value.kindDef == MOARANDROIDS.PawnKindDefOf.AbominationAtlas || value.kindDef == MOARANDROIDS.PawnKindDefOf.M7MechPawn)
            {
                value.Kill(null);
                return false;
            } else
            {
                return true;
            }
        }
    }
}
