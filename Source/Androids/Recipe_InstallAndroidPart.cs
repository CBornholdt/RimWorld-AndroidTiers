using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Recipe_InstallArtificialBodyPartAndroid : Recipe_SurgeryAndroids
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
            {
                BodyPartDef part = recipe.appliedOnFixedBodyParts[i];
                List<BodyPartRecord> bpList = pawn.RaceProps.body.AllParts;
                for (int j = 0; j < bpList.Count; j++)
                {
                    BodyPartRecord record = bpList[j];
                    if (record.def == part)
                    {
                        IEnumerable<Hediff> diffs = from x in pawn.health.hediffSet.hediffs
                                                    where x.Part == record
                                                    select x;
                        if (diffs.Count<Hediff>() != 1 || diffs.First<Hediff>().def != recipe.addsHediff)
                        {
                            if (record.parent == null || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(record.parent))
                            {
                                if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record))
                                {
                                    yield return record;
                                }
                            }
                        }
                    }
                }
            }
            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (base.CheckSurgeryFailAndroid(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
            }
            else if (pawn.Map != null)
            {
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, pawn.Position, pawn.Map);
            }
            else
            {
                pawn.health.RestorePart(part, null, true);
            }
            pawn.health.AddHediff(this.recipe.addsHediff, part, null);
        }
    }
}

namespace RimWorld
{
    // Token: 0x0200043D RID: 1085
    internal class MedicalRecipesUtility
    {
        // Token: 0x06001241 RID: 4673 RVA: 0x0008BF9F File Offset: 0x0008A39F
        public static bool IsCleanAndDroppable(Pawn pawn, BodyPartRecord part)
        {
            return !pawn.Dead && !pawn.RaceProps.Animal && part.def.spawnThingOnRemoved != null && MedicalRecipesUtility.IsClean(pawn, part);
        }

        // Token: 0x06001242 RID: 4674 RVA: 0x0008BFDC File Offset: 0x0008A3DC
        public static bool IsClean(Pawn pawn, BodyPartRecord part)
        {
            return !pawn.Dead && !(from x in pawn.health.hediffSet.hediffs
                                   where x.Part == part
                                   select x).Any<Hediff>();
        }

        // Token: 0x06001243 RID: 4675 RVA: 0x0008C02C File Offset: 0x0008A42C
        public static void RestorePartAndSpawnAllPreviousParts(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
        {
            MedicalRecipesUtility.SpawnNaturalPartIfClean(pawn, part, pos, map);
            MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part, pos, map);
            pawn.health.RestorePart(part, null, true);
        }

        // Token: 0x06001244 RID: 4676 RVA: 0x0008C04F File Offset: 0x0008A44F
        public static Thing SpawnNaturalPartIfClean(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
        {
            if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
            {
                return GenSpawn.Spawn(part.def.spawnThingOnRemoved, pos, map);
            }
            return null;
        }

        // Token: 0x06001245 RID: 4677 RVA: 0x0008C074 File Offset: 0x0008A474
        public static void SpawnThingsFromHediffs(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
        {
            if (!pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(part))
            {
                return;
            }
            IEnumerable<Hediff> enumerable = from x in pawn.health.hediffSet.hediffs
                                             where x.Part == part
                                             select x;
            foreach (Hediff hediff in enumerable)
            {
                if (hediff.def.spawnThingOnRemoved != null)
                {
                    GenSpawn.Spawn(hediff.def.spawnThingOnRemoved, pos, map);
                }
            }
            for (int i = 0; i < part.parts.Count; i++)
            {
                MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part.parts[i], pos, map);
            }
        }
    }
}
