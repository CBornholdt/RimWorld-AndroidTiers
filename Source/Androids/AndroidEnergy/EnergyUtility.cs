using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    static public class EnergyUtility
    {
		static public Hediff MakeGainEnergyHediff(Pawn pawn, float totalEnergy, int overTicks)
		{
			Hediff_GainEnergy gainEnergy = (Hediff_GainEnergy) HediffMaker.MakeHediff(EnergyHediffs.AT_GainEnergy, pawn);
			gainEnergy.totalEnergyGain = totalEnergy;
			gainEnergy.totalTickLength = Math.Max(overTicks, 1);
			gainEnergy.ticksPassed = 0;
			return gainEnergy;
		}
    }
}
