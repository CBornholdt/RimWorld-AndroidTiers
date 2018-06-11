using System.Reflection;
using Harmony;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.rwmods.androidtiers");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
