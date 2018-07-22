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

        static readonly public FleshTypeDef mechFlesh;

        static PawnExt()
        {
            androidFlesh = DefDatabase<FleshTypeDef>.GetNamed("Android");
            mechFlesh = DefDatabase<FleshTypeDef>.GetNamed("MechanisedInfantry");
        }

		static public bool IsRobot(this Pawn pawn)
		{
			return !pawn.RaceProps.IsFlesh || pawn.IsAndroid() || pawn.IsMech();
		}
    
        static public bool IsAndroid(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType == androidFlesh || pawn.IsMech();
        }
        
        static public bool IsMech(this Pawn pawn)
        {
        	return pawn.RaceProps.FleshType == mechFlesh;
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
