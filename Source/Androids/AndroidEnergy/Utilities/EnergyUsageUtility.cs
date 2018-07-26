using System;
using Verse;

namespace MOARANDROIDS
{
    static public class EnergyUsageUtility
    {
		static public float ExpectedUsagePerHour(Pawn pawn)
		{
			EnergySystem system = pawn.TryGetComp<EnergySystem>();

			if(system == null)
				return 0;

			float result = system.DesiredSinkRatePer1000Ticks - system.DesiredSourceRatePer1000Ticks;

			return (result > 0f) ? result * 2.5f : 0;   //2500 ticks per hour
		}

		static public float ExpectedUsagePerDay(Pawn pawn) => ExpectedUsagePerHour(pawn) * 24;
    }
}
