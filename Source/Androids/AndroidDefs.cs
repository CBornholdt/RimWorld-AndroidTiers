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
        static public JobDef AT_ConnectSink;
        static public JobDef AT_ConnectSinkThenDisconnect;
	}

	[DefOf]
	static public class EnergyHediffs
	{
		static public HediffDef AT_GainEnergy;
		static public HediffDef AT_NotPresent;
	}

	[DefOf]
	static public class AndroidParts
	{
		static public BodyPartDef MEnergySystem;
		static public BodyPartDef MConduit;
		static public BodyPartDef MReactor;
		static public BodyPartDef MBattery;
	}
}
