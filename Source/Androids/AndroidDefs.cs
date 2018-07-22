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

	[DefOf]
	static public class AndroidImplants
	{
		static public ThingDef AT_MindCaptureImplantEmpty;
		static public ThingDef AT_MindCaptureImplantFull;
	}

	[DefOf]
	static public class MiscJobs
	{
        static public JobDef AT_CarryToMindUploadCasket;
        static public JobDef AT_EnterMindUploadCasket;
		static public JobDef AT_StartMindUpload;
	}

	[DefOf]
	static public class AndroidBuildings
	{
		static public ThingDef AT_MindUploadCasket;
	}
}
