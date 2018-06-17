using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class EnergySystemProps_Battery : CompProperties
    {
		public float sinkPriority = 1;
		public float sourcePriority = 10;
		public float storagePriority = 10;
        public float attachPriority = 5;

		public float storageCapacity = 1f;
		public float energyTransferPer1000Ticks = 1;

		public float initialStoragePercentage = 1;
        
		public EnergySystemProps_Battery() => this.compClass = typeof(EnergySystemComp_Battery);
    }
}
