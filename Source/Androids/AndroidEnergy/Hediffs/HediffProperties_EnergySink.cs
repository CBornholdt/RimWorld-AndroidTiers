using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffProperties_EnergySink : HediffCompProperties
    {
		public float sinkPriority = 1f;
		public float desiredSinkRatePer1000Ticks = 0.1f;
		public bool activeSink = true;
    
        public HediffProperties_EnergySink()
        {
			this.compClass = typeof(HediffComp_EnergySink);
        }
    }
}
