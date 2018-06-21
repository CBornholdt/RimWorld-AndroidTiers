using System;
using RimWorld;
using Verse;
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

		static public StorageLevelTag GetDefaultStorageLevel(this IEnergyStorage storage)
		{
			float fullRatio = storage.StoredEnergy / storage.StorageCapacity;
			if(fullRatio < 0.000001)
				return StorageLevelTag.Empty;
			if(fullRatio < 0.1)
				return StorageLevelTag.CriticallyLow;
			if(fullRatio < 0.25)
				return StorageLevelTag.Low;
			if(fullRatio > 0.99999)
				return StorageLevelTag.Full;
			return StorageLevelTag.Normal;
		}

		static public IntVec3 Position(this IEnergySource source)
		{
			if(source is Thing thing)
				return thing.PositionHeld;
			if(source is ThingComp comp)
				return comp.parent.PositionHeld;
			return IntVec3.Invalid;
		}
        
        static public IntVec3 Position(this IEnergySink sink)
        {
            if(sink is Thing thing)
                return thing.PositionHeld;
            if(sink is ThingComp comp)
                return comp.parent.PositionHeld;
            return IntVec3.Invalid;
        }
    }
}
