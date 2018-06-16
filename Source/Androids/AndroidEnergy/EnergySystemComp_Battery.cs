using System;
using Verse;
using UnityEngine;
using Harmony;

namespace MOARANDROIDS
{
    public class EnergySystemComp_Battery : EnergySystemComp, IEnergyStorage
    {
		private float storedEnergy;

		public EnergySystemProps_Battery Props => (EnergySystemProps_Battery) this.props;

		public override void PostExposeData()
		{
			Scribe_Values.Look<float>(ref this.storedEnergy, "StoredEnergy");
            
            if(Scribe.mode == LoadSaveMode.LoadingVars)
                Traverse.Create(Scribe.loader.crossRefs).Field("loadedObjectDirectory").Method("RegisterLoaded", new object[1] { this });
		}

		override public float AttachPriority => this.Props.attachPriority;
        
		public SinkStatusType SinkStatus => (StoredEnergy >= StorageCapacity) ? SinkStatusType.NotActive : SinkStatusType.Active;
		public float SinkPriority => this.Props.sinkPriority;
		public float DesiredSinkRatePer1000Ticks => this.Props.energyTransferPer1000Ticks;
		public float TrySinkEnergy(float amount)
		{
			float amountToSink = Mathf.Clamp(amount, 0, StorageCapacity - StoredEnergy);
			storedEnergy += amountToSink;
			return amountToSink;
		}

		public SourceStatusType SourceStatus => (StoredEnergy <= 0) ? SourceStatusType.NotActive : SourceStatusType.Active;
		public float SourcePriority => this.Props.sourcePriority;
		public float DesiredSourceRatePer1000Ticks => this.Props.energyTransferPer1000Ticks;
		public float TrySourceEnergy(float amount)
		{
			float amountToSource = Mathf.Clamp(amount, 0, StoredEnergy);
			storedEnergy -= amountToSource;
			return amountToSource;
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
