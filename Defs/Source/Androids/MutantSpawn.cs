using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace MOARANDROIDS
{
    public class Hediff_Fractal : HediffWithComps
    { 
            public override string LabelInBrackets
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(base.LabelInBrackets);
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            string compLabelInBracketsExtra = this.comps[i].CompLabelInBracketsExtra;
                            if (!compLabelInBracketsExtra.NullOrEmpty())
                            {
                                if (stringBuilder.Length != 0)
                                {
                                    stringBuilder.Append(", ");
                                }
                                stringBuilder.Append(compLabelInBracketsExtra);
                            }
                        }
                    }
                    return stringBuilder.ToString();
                }
            }

            // Token: 0x17000A83 RID: 2691
            // (get) Token: 0x0600431E RID: 17182 RVA: 0x0008B540 File Offset: 0x00089940
            public override bool ShouldRemove
            {
                get
                {
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            if (this.comps[i].CompShouldRemove)
                            {
                                return true;
                            }
                        }
                    }
                    return base.ShouldRemove;
                }
            }

            // Token: 0x17000A84 RID: 2692
            // (get) Token: 0x0600431F RID: 17183 RVA: 0x0008B594 File Offset: 0x00089994
            public override bool Visible
            {
                get
                {
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            if (this.comps[i].CompDisallowVisible())
                            {
                                return false;
                            }
                        }
                    }
                    return base.Visible;
                }
            }

            // Token: 0x17000A85 RID: 2693
            // (get) Token: 0x06004320 RID: 17184 RVA: 0x0008B5E8 File Offset: 0x000899E8
            public override string TipStringExtra
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(base.TipStringExtra);
                    if (this.comps != null)
                    {
                        for (int i = 0; i < this.comps.Count; i++)
                        {
                            string compTipStringExtra = this.comps[i].CompTipStringExtra;
                            if (!compTipStringExtra.NullOrEmpty())
                            {
                                stringBuilder.AppendLine(compTipStringExtra);
                            }
                        }
                    }
                    return stringBuilder.ToString();
                }
            }

            // Token: 0x17000A86 RID: 2694
            // (get) Token: 0x06004321 RID: 17185 RVA: 0x0008B65C File Offset: 0x00089A5C
            public override TextureAndColor StateIcon
            {
                get
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        TextureAndColor compStateIcon = this.comps[i].CompStateIcon;
                        if (compStateIcon.HasValue)
                        {
                            return compStateIcon;
                        }
                    }
                    return TextureAndColor.None;
                }
            }

            // Token: 0x06004322 RID: 17186 RVA: 0x0008B6AC File Offset: 0x00089AAC
            public override void PostAdd(DamageInfo? dinfo)
            {
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostPostAdd(dinfo);
                    }
                }
            }

            // Token: 0x06004323 RID: 17187 RVA: 0x0008B6F4 File Offset: 0x00089AF4
            public override void PostRemoved()
            {
                base.PostRemoved();
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostPostRemoved();
                    }
                }
            }

            // Token: 0x06004324 RID: 17188 RVA: 0x0008B740 File Offset: 0x00089B40
            public override void PostTick()
            {
                base.PostTick();
                if (this.comps != null)
                {
                    float num = 0f;
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostTick(ref num);
                    }
                    if (num != 0f)
                    {
                        this.Severity += num;
                    }
                }
            }

            // Token: 0x06004325 RID: 17189 RVA: 0x0008B7AC File Offset: 0x00089BAC
            public override void ExposeData()
            {
                base.ExposeData();
                if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    this.InitializeComps();
                }
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompExposeData();
                    }
                }
            }

            // Token: 0x06004326 RID: 17190 RVA: 0x0008B808 File Offset: 0x00089C08
            public override void Tended(float quality, int batchPosition = 0)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompTended(quality, batchPosition);
                }
            }

            // Token: 0x06004327 RID: 17191 RVA: 0x0008B844 File Offset: 0x00089C44
            public override bool TryMergeWith(Hediff other)
            {
                if (base.TryMergeWith(other))
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        this.comps[i].CompPostMerged(other);
                    }
                    return true;
                }
                return false;
            }

            // Token: 0x06004328 RID: 17192 RVA: 0x0008B890 File Offset: 0x00089C90
            public override void Notify_PawnDied()
            {
                base.Notify_PawnDied();
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].Notify_PawnDied();
                }
            }

            // Token: 0x06004329 RID: 17193 RVA: 0x0008B8D0 File Offset: 0x00089CD0
            public override void ModifyChemicalEffect(ChemicalDef chem, ref float effect)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompModifyChemicalEffect(chem, ref effect);
                }
            }

            // Token: 0x0600432A RID: 17194 RVA: 0x0008B90C File Offset: 0x00089D0C
            public override void PostMake()
            {
                base.PostMake();
                this.InitializeComps();
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompPostMake();
                }
            }

            // Token: 0x0600432B RID: 17195 RVA: 0x0008B954 File Offset: 0x00089D54
            private void InitializeComps()
            {
                if (this.def.comps != null)
                {
                    this.comps = new List<HediffComp>();
                    for (int i = 0; i < this.def.comps.Count; i++)
                    {
                        HediffComp hediffComp = (HediffComp)Activator.CreateInstance(this.def.comps[i].compClass);
                        hediffComp.props = this.def.comps[i];
                        hediffComp.parent = this;
                        this.comps.Add(hediffComp);
                    }
                }
            }

            // Token: 0x0600432C RID: 17196 RVA: 0x0008B9E8 File Offset: 0x00089DE8
            public override string DebugString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(base.DebugString());
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        string str;
                        if (this.comps[i].ToString().Contains("_"))
                        {
                            str = this.comps[i].ToString().Split(new char[]
                            {
                            '_'
                            })[1];
                        }
                        else
                        {
                            str = this.comps[i].ToString();
                        }
                        stringBuilder.AppendLine("--" + str);
                        string text = this.comps[i].CompDebugString();
                        if (!text.NullOrEmpty())
                        {
                            stringBuilder.AppendLine(text.TrimEnd(new char[0]).Indented());
                        }
                    }
                }
                return stringBuilder.ToString();
            }

    private bool IsSeverelyWounded
        {
            get
            {
                float num = 0f;
                List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i] is Hediff_Injury && !hediffs[i].IsOld())
                    {
                        num += hediffs[i].Severity;
                    }
                }
                List<Hediff_MissingPart> missingPartsCommonAncestors = this.pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                for (int j = 0; j < missingPartsCommonAncestors.Count; j++)
                {
                    if (missingPartsCommonAncestors[j].IsFreshNonSolidExtremity)
                    {
                        num += missingPartsCommonAncestors[j].Part.def.GetMaxHealth(this.pawn);
                    }
                }
                return num > 38f * this.pawn.RaceProps.baseHealthScale;
            }
        }

        // Token: 0x06004362 RID: 17250 RVA: 0x001E7FF8 File Offset: 0x001E63F8
        public override void Tick()
        {
            this.ageTicks++;
            if (this.Severity >= 1f)
            {
                if (this.Visible && PawnUtility.ShouldSendNotificationAbout(this.pawn))
                {
                    Messages.Message("ColonistMutated".Translate(new object[]
                    {
                            this.pawn.LabelIndefinite()
                    }).CapitalizeFirst(), this.pawn, MessageTypeDefOf.ThreatBig);
                }
                Hediff_Fractal.DoMutation(this.pawn);
                this.pawn.Destroy();
            }
        }

        public static void DoMutation(Pawn premutant)
        {
            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.AbominationAtlas, Faction.OfMechanoids, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            FilthMaker.MakeFilth(premutant.Position, premutant.Map, RimWorld.ThingDefOf.FilthAmnioticFluid, premutant.LabelIndefinite(), 10);
            FilthMaker.MakeFilth(premutant.Position, premutant.Map, RimWorld.ThingDefOf.FilthBlood, premutant.LabelIndefinite(), 10);

            GenSpawn.Spawn(pawn, premutant.Position, premutant.Map);
            pawn.mindState.mentalStateHandler.TryStartMentalState(RimWorld.MentalStateDefOf.ManhunterPermanent, null, true, false, null);
        }
    }
}
