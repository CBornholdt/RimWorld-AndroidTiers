using System;
using Verse;
using UnityEngine;
using Harmony;

namespace MOARANDROIDS
{
    public class EnergySystemComp_Battery : EnergySystemComp, IEnergyStorage
    {
		float storedEnergy;
    
		public EnergySystemProps_Battery Props => (EnergySystemProps_Battery) this.props;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			if(Scribe.mode == LoadSaveMode.Inactive)
				PostPostMake();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.storedEnergy, "StoredEnergy");
		}

		public void PostPostMake()
		{
			this.storedEnergy = Props.initialStoragePercentage * StorageCapacity;
		}

		override public float AttachPriority => this.Props.attachPriority;
        
		public SinkStatusType SinkStatus => (StoredEnergy >= StorageCapacity) ? SinkStatusType.Disabled : SinkStatusType.Passive;
		public float SinkPriority => this.Props.sinkPriority;
		public float DesiredSinkRatePer1000Ticks => this.Props.energyTransferPer1000Ticks;
		public float CurrentMaxSinkableEnergy => StorageCapacity - StoredEnergy;
		public void SinkEnergy(float amount)
		{
			storedEnergy += amount;
			storedEnergy = Mathf.Min(storedEnergy, StorageCapacity);
		}

		public SourceStatusType SourceStatus => (StoredEnergy <= 0) ? SourceStatusType.Disabled : SourceStatusType.Passive;
		public float SourcePriority => this.Props.sourcePriority;
		public float DesiredSourceRatePer1000Ticks => this.Props.energyTransferPer1000Ticks;
		public float CurrentMaxSourcableEnergy => StoredEnergy;
		public void SourceEnergy(float amount)
		{
			storedEnergy -= amount;
			storedEnergy = Mathf.Max(storedEnergy, 0);
		}
        
        public StorageStatusType StorageStatus {
			get {
				if(StoredEnergy <= 0) {
					if(StorageCapacity <= 0)
						return StorageStatusType.NotActive;
					return StorageStatusType.Empty;
				}
				if(StoredEnergy >= StorageCapacity)
					return StorageStatusType.Full;
				return StorageStatusType.Active;
			}
		}

		public float StoragePriority => this.Props.storagePriority;
		public float StoredEnergy => this.storedEnergy;
		public float StorageCapacity => this.Props.storageCapacity;

		public string GetUniqueLoadID() => this.parent.GetUniqueLoadID() + "_ESC_Battery";
    }
}
