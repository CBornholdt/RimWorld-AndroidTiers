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
    [HarmonyPatch(typeof(JobGiver_PatientGoToBed))]
    [HarmonyPatch(nameof(JobGiver_PatientGoToBed.TryIssueJobPackage))]
    static public class JobGiver_PatientGoToBed_TryIssueJobPackage
    {
		static bool Prefix(Pawn pawn, ThinkResult __result)
		{
			if(pawn.IsMech()) {
				__result = ThinkResult.NoJob;
				return false;
			}
			return true;
		}
    }
}
