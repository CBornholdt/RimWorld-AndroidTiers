using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [Flags]
    public enum DisconnectWhenTag
    {
        Never = 0,
        SinkDisabled = 0x1,
        SinkPassive = 0x2,
        SinkActive = 0x4,
        SinkSlow = 0x8,
        SinkLow = 0x10,
        SourceDisabled = 0x20,
        SourcePassive = 0x40,
        SourceActive = 0x80,
        SourceSlow = 0x100,
        SourceLow = 0x200, 
        StorageEmpty = 0x400,
        StorageCriticallyLow = 0x800,
        StorageLow = 0x1000,
        StorageNormal = 0x2000,
        StorageFull = 0x4000,
        Touching = 0x8000,
        NotTouching = 0x10000,
        Time = 0x20000,       
        EnergySystemEmpty = 0x40000,
        EnergySystemFull = 0x80000
    }

    public struct DisconnectWhen
    {
        public DisconnectWhenTag whenTag;
        public int atTick;

        public DisconnectWhen(DisconnectWhenTag tag = DisconnectWhenTag.Never, int atTick = 0)
        {
            this.whenTag = tag;
            this.atTick = atTick;
        }
    }

	public static class DisconnectWhenExt
	{
		public static bool ShouldDisconnectSystemFrom(this DisconnectWhen when, EnergySystem system, IEnergySource source)
		{
			DisconnectWhenTag tag = when.whenTag;
			if(tag == DisconnectWhenTag.Never)
				return false;
			if((tag & DisconnectWhenTag.SourceDisabled) != 0 && source.SourceStatus == SourceStatusType.Disabled)
				return true;
			if((tag & DisconnectWhenTag.SourcePassive) != 0 && source.SourceStatus == SourceStatusType.Passive)
				return true;
			if((tag & DisconnectWhenTag.SourceActive) != 0 && source.SourceStatus == SourceStatusType.Active)
				return true;
			if((tag & DisconnectWhenTag.SourceSlow) != 0) {
				int ticks = Find.TickManager.TicksGame - system.LastTickProcessed;
				float desiredMaxOutput = ticks * source.DesiredSourceRatePer1000Ticks / 1000f;
				if(source.CurrentMaxSourcableEnergy / desiredMaxOutput < EnergyConstants.EnergySourceSlowThresh)
					return true;
			}
			if((tag & DisconnectWhenTag.SourceLow) != 0 && source.SourceLevelPercent() <= EnergyConstants.EnergySourceLowThresh)
				return true;
			
			if((tag & DisconnectWhenTag.Touching) != 0 && source.Position().AdjacentTo8WayOrInside(system.parent.PositionHeld))
				return true;
			if((tag & DisconnectWhenTag.NotTouching) != 0 && !source.Position().AdjacentTo8WayOrInside(system.parent.PositionHeld))
				return true;

			if(when.SystemAndOtherChecks(system))
				return true;

			IEnergyStorage storage = source as IEnergyStorage;
			if(storage != null && when.StorageChecks(system, storage))
				return true;

			return false;
		}

		private static bool SystemAndOtherChecks(this DisconnectWhen when, EnergySystem system)
		{
			if((when.whenTag & DisconnectWhenTag.Time) != 0 && when.atTick <= Find.TickManager.TicksGame)
				return true;

			if((when.whenTag & DisconnectWhenTag.EnergySystemFull) != 0 && system.StorageLevel == StorageLevelTag.Full)
				return true;

			if((when.whenTag & DisconnectWhenTag.EnergySystemEmpty) != 0 && system.StorageLevel == StorageLevelTag.Empty)
				return true;
			return false;
		}

		private static bool StorageChecks(this DisconnectWhen when, EnergySystem system, IEnergyStorage storage)
		{
			StorageLevelTag storageLevel = storage.StorageLevel;
			if((when.whenTag & DisconnectWhenTag.StorageEmpty) != 0 && storageLevel == StorageLevelTag.Empty)
				return true;
			if((when.whenTag & DisconnectWhenTag.StorageCriticallyLow) != 0 && storageLevel == StorageLevelTag.CriticallyLow)
				return true;
			if((when.whenTag & DisconnectWhenTag.StorageLow) != 0 && storageLevel == StorageLevelTag.Low)
				return true;
			if((when.whenTag & DisconnectWhenTag.StorageNormal) != 0 && storageLevel <= StorageLevelTag.Normal)
				return true;
			if((when.whenTag & DisconnectWhenTag.StorageFull) != 0 && storageLevel <= StorageLevelTag.Full)
				return true;
			return false;
		}

		public static bool ShouldDisconnectSystemFrom(this DisconnectWhen when, EnergySystem system, IEnergySink sink)
		{
			DisconnectWhenTag tag = when.whenTag;
			if(tag == DisconnectWhenTag.Never)
				return false;
			if((tag & DisconnectWhenTag.SinkDisabled) != 0 && sink.SinkStatus == SinkStatusType.Disabled)
				return true;
			if((tag & DisconnectWhenTag.SinkPassive) != 0 && sink.SinkStatus == SinkStatusType.Passive)
				return true;
			if((tag & DisconnectWhenTag.SinkActive) != 0 && sink.SinkStatus == SinkStatusType.Active)
				return true;
			if((tag & DisconnectWhenTag.SinkSlow) != 0) {
				int ticks = Find.TickManager.TicksGame - system.LastTickProcessed;
				float desiredMaxOutput = ticks * sink.DesiredSinkRatePer1000Ticks / 1000f;
				if(sink.CurrentMaxSinkableEnergy / desiredMaxOutput < EnergyConstants.EnergySourceSlowThresh)
					return true;
			}

			if((tag & DisconnectWhenTag.Touching) != 0 && sink.Position().AdjacentTo8WayOrInside(system.parent.PositionHeld))
				return true;
			if((tag & DisconnectWhenTag.NotTouching) != 0 && !sink.Position().AdjacentTo8WayOrInside(system.parent.PositionHeld))
				return true;

			if(when.SystemAndOtherChecks(system))
				return true;

			IEnergyStorage storage = sink as IEnergyStorage;
			if(storage != null && when.StorageChecks(system, storage))
				return true;

			return false;
		}
	}
}
