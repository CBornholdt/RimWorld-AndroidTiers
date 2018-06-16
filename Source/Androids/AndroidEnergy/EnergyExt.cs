using System;
namespace MOARANDROIDS
{
    static public class EnergyExt
    {
		static public bool ActiveStorageOrFull(this IEnergyStorage storage) =>
			storage.StorageStatus == StorageStatusType.Active
			|| storage.StorageStatus == StorageStatusType.Full;
            
        static public bool ActiveStorageOrEmpty(this IEnergyStorage storage) =>
            storage.StorageStatus == StorageStatusType.Active
            || storage.StorageStatus == StorageStatusType.Empty;    
    }
}
