using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
	public class CompProperties_NeedsEnergy : CompProperties
    {
		public int ticksToLoseTenPercent = 10000;
        
		public float lowLevelThreshPercent = 0.25f;
		public float criticallyLowLevelThreshPercent = 0.1f;
		public float powerForFullEnergy = 800;

		public HediffDef lowLevelHediff;
		public HediffDef criticallyLowLevelHediff;
		public HediffDef emptyLevelHediff;

		public float ValueLossPerTick => 0.1f / (float)ticksToLoseTenPercent;
        
        public CompProperties_NeedsEnergy() => this.compClass = typeof(CompNeedsEnergy);
    }
}
