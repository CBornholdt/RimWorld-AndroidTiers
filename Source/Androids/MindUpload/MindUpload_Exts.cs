using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    static public class MindUpload_Exts
    {
		static public bool IsValidForMindUpload(this Pawn pawn) =>
			pawn.RaceProps.intelligence >= Intelligence.Humanlike
				&& !pawn.IsRobot();     
    }
}
