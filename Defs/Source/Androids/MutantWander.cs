using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x02000327 RID: 807
    public class IncidentWorker_MutantJoin : IncidentWorker
    {
        // Token: 0x06000D69 RID: 3433 RVA: 0x00062EE4 File Offset: 0x000612E4
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Neutral, out loc))
            {
                return false;
            }
            PawnKindDef pawnKindDef = new List<PawnKindDef>
            {
                RimWorld.PawnKindDefOf.Villager
            }.RandomElement<PawnKindDef>();
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 20f, false, true, true, false, false, false, false, null, null, null, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(pawn, loc, map);
            string text = "WandererJoin".Translate(new object[]
            {
                pawnKindDef.label,
                pawn.story.Title.ToLower()
            });
            text = text.AdjustedFor(pawn);
            string label = "LetterLabelWandererJoin".Translate();
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, pawn, null);
            GiveMutantHediff(pawn);
            return true;
        }

        protected static void GiveMutantHediff(Pawn infected)
        {
            Hediff hediff = HediffMaker.MakeHediff(MOARANDROIDS.HediffDefOf.Conversion, infected, null);
            hediff.Severity = 0.01f;
            infected.health.AddHediff(hediff, null, null);
        }
        // Token: 0x0400089A RID: 2202
        private const float RelationWithColonistWeight = 20f;
    }
}
