using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;

//Extension method, in RimWorld namespace
namespace RimWorld
{ 
    //Make sure I bind to the Android fleshDef as early as possible AFTER they are loaded
    [StaticConstructorOnStartup]
    static public class PawnExt
    {
        static readonly public FleshTypeDef androidFlesh;

        static PawnExt()
        {
            androidFlesh = DefDatabase<FleshTypeDef>.GetNamed("Android");
        }
    
        static public bool IsAndroid(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType == androidFlesh;
        }

		static public int RemoveAllHediffsWhere(this Pawn pawn, Func<Hediff, bool> hediffChoser)
		{
			var hediffsToRemove = pawn.health?.hediffSet.hediffs.Where(hediffChoser).ToList() ?? new List<Hediff>();

			foreach(var hediff in hediffsToRemove)
				pawn.health.RemoveHediff(hediff);

			return hediffsToRemove.Count;
		}
    }
}
