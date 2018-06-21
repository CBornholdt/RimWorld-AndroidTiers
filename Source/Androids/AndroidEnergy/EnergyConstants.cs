using System;
namespace MOARANDROIDS
{
    static public class EnergyConstants
    {
        static public readonly float EnergyThreshPercentAcceptable = 0.8f;
        static public readonly float AdditionalRechargeThresholdFromLow = 0.25f;
		static public readonly int TicksPerSystemUpdate = 25;
		static public readonly float EnergySourceSlowThresh = 0.2f;
		static public readonly float EnergySinkSlowThresh = 0.2f;
		static public readonly float EnergySourceLowThresh = 0.15f;
		static public readonly float EnergySinkLowThresh = 0.15f;
		static public readonly float BatteryRechargeNormalThresh = 0.2f;
    }
}
