using System;
namespace MOARANDROIDS
{
	public enum StorageStatusType : byte { NotActive, Empty, Active, Full };

    public interface IEnergyStorage : IEnergySink, IEnergySource
    {
        StorageStatusType StorageStatus { get; }
        float StoragePriority { get; }
        float StoredEnergy { get; }
        float StorageCapacity { get; }
    }
}
