using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class EnergyAdapter_PowerBatteryProps : CompProperties_Battery
    {
		public float sourcePriority = 10f;
		public float desiredTranferRatePer1000Ticks = 1;
		public float powerPerUnitEnergy = 800f;
    
		public EnergyAdapter_PowerBatteryProps() => this.compClass = typeof(EnergyAdapter_PowerBattery);
    }
}
