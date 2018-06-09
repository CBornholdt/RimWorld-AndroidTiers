using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public static class DownedT5Utility
    {
        public static Pawn GenerateT5(int tile)
        {
            PawnKindDef AndroidT5Colonist = PawnKindDefOf.AndroidT5Colonist;
            Faction ofplayer = Faction.OfSpacer;
            PawnGenerationRequest request = new PawnGenerationRequest(AndroidT5Colonist, ofplayer, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 20f, false, true, true, false, false, false, false, null, new float?(0.2f), null, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            HealthUtility.DamageUntilDowned(pawn);
            return pawn;
        }
        private const float RelationWithColonistWeight = 0.8f;

        private const float ChanceToRedressWorldPawn = 0f;
    }
}