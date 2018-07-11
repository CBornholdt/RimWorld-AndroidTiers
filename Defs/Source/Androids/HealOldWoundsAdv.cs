using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class HediffComp_HealOldWoundsAdv : HediffComp
    {

        public HediffCompProperties_HealOldWoundsAdv Props
        {
            get
            {
                return (HediffCompProperties_HealOldWoundsAdv)this.props;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.ResetTicksToHeal();
        }

        private void ResetTicksToHeal()
        {
            this.ticksToHeal = Rand.Range(5, 20) * 1000;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            this.ticksToHeal--;
            if (this.ticksToHeal <= 0)
            {
                this.TryHealRandomOldWound();
                this.ResetTicksToHeal();
            }
        }

        private void TryHealRandomOldWound()
        {
            IEnumerable<Hediff> hediffs = base.Pawn.health.hediffSet.hediffs;
            if (HediffComp_HealOldWoundsAdv.stuff == null)
			{
                HediffComp_HealOldWoundsAdv.stuff = new Func<Hediff, bool>(HediffUtility.IsOld);
            }
            Hediff hediff;
            if (!hediffs.Where(HediffComp_HealOldWoundsAdv.stuff).TryRandomElement(out hediff))
            {
                return;
            }
            hediff.Severity = 0f;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticksToHeal, "ticksToHeal", 0, false);
        }

        public override string CompDebugString()
        {
            return "ticksToHeal: " + this.ticksToHeal;
        }

        private int ticksToHeal;

        [CompilerGenerated]
        private static Func<Hediff, bool> stuff;
	}
}
