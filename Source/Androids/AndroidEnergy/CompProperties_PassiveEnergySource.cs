using System;
using Verse;
using RimWorld;


namespace MOARANDROIDS
{
    public class CompProperties_PassiveEnergySource : CompProperties
    {
		public float energyPer1000Ticks = 0.1f;
		public int maximumTickInterval = 150;
    
        public CompProperties_PassiveEnergySource()
        {
			this.compClass = typeof(Comp_PassiveEnergySource);
        }
    }
}
