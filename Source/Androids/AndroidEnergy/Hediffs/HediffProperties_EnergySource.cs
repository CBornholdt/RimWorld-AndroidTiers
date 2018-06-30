using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffProperties_EnergySource : HediffCompProperties
    {
        public float sourcePriority = 10f;
        public float desiredSourceRatePer1000Ticks = 0.1f;
        public bool activeSource = true;
    
        public HediffProperties_EnergySource()
        {
            this.compClass = typeof(HediffComp_EnergySource);
        }
    }
}
