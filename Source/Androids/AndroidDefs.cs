using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [DefOf]
    static public class AndroidNeeds
    {
		static public NeedDef AT_Energy;
    }

	[DefOf]
	static public class EnergyJobs
	{
		static public JobDef AT_RechargeEnergy;
		static public JobDef AT_ConnectSource;
		static public JobDef AT_ConnectSourceThenDisconnect;
	}

	[DefOf]
	static public class EnergyHediffs
	{
		static public HediffDef AT_GainEnergy;
	}
}
