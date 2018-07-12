using System;
using Verse;

namespace MOARANDROIDS
{
    public class EnergySearchSettings : IExposable
    {
        public bool allowConsumables = true;
        public bool allowPowerBatteries = true;
        public float consumableDistanceBias = 1f;
        public float batteryProtectionLevel = 0.1f;
        public bool allowCriticalToOverrideProtection = true;
        public int batteryReconnectionTicks = 250;
        public bool allowConsumableWaste = false;
		public int maxConsumablesToCarry = 3;

        public void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.allowConsumables, "AllowConsumables");
            Scribe_Values.Look<bool>(ref this.allowPowerBatteries, "AllowPowerBatteries");
            Scribe_Values.Look<float>(ref this.consumableDistanceBias, "ConsumableDistanceBias");
            Scribe_Values.Look<float>(ref this.batteryProtectionLevel, "BatteryProtectionLevel");
            Scribe_Values.Look<bool>(ref this.allowCriticalToOverrideProtection, "AllowCriticalToOverrideProtection");
            Scribe_Values.Look<int>(ref this.batteryReconnectionTicks, "BatteryReconnectionTicks");
            Scribe_Values.Look<bool>(ref this.allowConsumableWaste, "AllowConsumableWaste");
			Scribe_Values.Look<int>(ref this.maxConsumablesToCarry, "MaxConsumablesToCarry");
        }
    }
}
