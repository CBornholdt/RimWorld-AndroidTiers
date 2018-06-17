using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class CompPowerBattery_EnergyAdaptorProps : CompProperties_Battery
    {
		public float sourcePriority = 10f;
		public float desiredTranferRatePer1000Ticks = 1;
		public float powerPerUnitEnergy = 800f;
    
		public CompPowerBattery_EnergyAdaptorProps() => this.compClass = typeof(CompPowerBattery_EnergyAdapterComp);
    }
}
