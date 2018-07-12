using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffProperties_EnergyStorage : HediffCompProperties
    {
        public float sourcePriority = 1f;
		public float sinkPriority = 1f;
		public float storagePriority = 1f;
		public float storageCapacity = 1f;
        public float desiredTransferRatePer1000Ticks = 5f;
    
        public HediffProperties_EnergyStorage()
        {
            this.compClass = typeof(HediffComp_EnergyStorage);
        }
    }
}
