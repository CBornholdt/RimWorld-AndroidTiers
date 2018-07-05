using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
	public enum SourceStatusType : byte { Disabled = 0, Passive = 1, Active = 2 };

    public interface IEnergySource : ILoadReferenceable
    {
		SourceStatusType SourceStatus { get; }
        float SourcePriority { get; }
        float DesiredSourceRatePer1000Ticks { get; }
        float CurrentMaxSourcableEnergy { get; }
		void SourceEnergy(float amount);
		void SourceAttached(EnergySystem system);
		void SourceDetached(EnergySystem system);
    }

	static public class SourceExt
	{
		static public float SourceLevelPercent(this IEnergySource source)
		{
			if(source is IEnergyStorage storage)
				return storage.StoredEnergy / storage.StorageCapacity;
			if(source is EnergyAdapter_PowerBattery battery)
				return battery.StoredEnergyPct;
			return 1f;
		}
	}
}
