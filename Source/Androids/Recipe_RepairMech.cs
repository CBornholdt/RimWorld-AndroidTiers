using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x0200003A RID: 58
    public class Recipe_RepairMechInfantry : RecipeWorker
    {
        // Token: 0x060000F2 RID: 242 RVA: 0x00009902 File Offset: 0x00007B02
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            bool flag = pawn.def.HasModExtension<AndroidTweaker>();
            if (flag)
            {
                bool flag2;
                if (pawn.health.hediffSet.BleedRateTotal <= 0f && pawn.health.summaryHealth.SummaryHealthPercent >= 1f && pawn.health.hediffSet.GetMissingPartsCommonAncestors().Count <= 0)
                {
                    flag2 = pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.def.HasModExtension<AndroidTweaker>() && hediff.CurStage.becomeVisible);
                }
                else
                {
                    flag2 = true;
                }
                bool flag3 = flag2;
                if (flag3)
                {
                    yield return null;
                }
            }
            else
            {
                bool flag4 = pawn.health.hediffSet.BleedRateTotal > 0f;
                if (flag4)
                {
                    yield return null;
                }
            }
            yield break;
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x00009920 File Offset: 0x00007B20
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.AndroidCoolantLoss, false);
            bool flag = firstHediffOfDef != null;
            if (flag)
            {
                pawn.health.RemoveHediff(firstHediffOfDef);
            }
            bool flag2 = pawn.def.HasModExtension<AndroidTweaker>();
            if (flag2)
            {
                List<Hediff> list = new List<Hediff>();
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    bool flag3 = hediff is Hediff_MissingPart || hediff is Hediff_Injury || hediff.def.HasModExtension<AndroidTweaker>();
                    if (flag3)
                    {
                        list.Add(hediff);
                    }
                }
                foreach (Hediff hediff2 in list)
                {
                    pawn.health.RemoveHediff(hediff2);
                }
            }
        }
    }
}
