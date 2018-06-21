using System;
namespace MOARANDROIDS
{
	public enum StorageStatusType : byte { NotActive, Empty, Active, Full };
	public enum StorageLevelTag : byte { Empty, CriticallyLow, Low, Normal, Full };

    public interface IEnergyStorage : IEnergySink, IEnergySource
    {
        StorageStatusType StorageStatus { get; }
        float StoragePriority { get; }
        float StoredEnergy { get; }
		void SetEnergyDirect(float amount);
        float StorageCapacity { get; }
        StorageLevelTag StorageLevel { get; }
        
    }
}
