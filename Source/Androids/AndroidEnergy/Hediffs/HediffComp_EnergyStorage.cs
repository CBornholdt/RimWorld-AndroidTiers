using System;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
    public class HediffComp_EnergyStorage : HediffComp_AndroidImplant, IEnergyStorage
    {
        public HediffProperties_EnergyStorage Props => (HediffProperties_EnergyStorage)this.props;

        public EnergySystem EnergySystem => this.Pawn.TryGetComp<EnergySystem>();

        int lastTickWorked;
		float storedEnergy;

		bool wasLastSourcedFromNotSinked = false;
		float hourlyPercentageChange = 0f;
    
        public HediffComp_EnergyStorage()
        {
			this.storedEnergy = 0;
			this.lastTickWorked = Find.TickManager.TicksGame - 1;
        }
        
        public override string CompTipStringExtra {
			get {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("AT.HediffComp.StoredEnergy.TipString"
											.Translate(StoredEnergy, StorageCapacity));
				string chargingOrDischarging = wasLastSourcedFromNotSinked ? "Discharging" : "Charging";
				stringBuilder.AppendLine(("AT.HediffComp.PercentChanged." + chargingOrDischarging + ".TipString")
											.Translate(hourlyPercentageChange.ToStringPercent()));
				return stringBuilder.ToString();
			}
		}      

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			if(EnergySystem == null) {
				Log.ErrorOnce($"Somehow an energy hediff was placed onto a pawn without an energy system", 82349);
				return;
			}
        
			EnergySystem.AttachSource(this);
			EnergySystem.AttachSink(this);
		}

		public override void CompPostPostRemoved()
		{
            if(EnergySystem == null) {
                Log.ErrorOnce($"Somehow an energy hediff was placed onto a pawn without an energy system", 82350);
                return;
            }
        
			EnergySystem.DetachSource(this);
			EnergySystem.DetachSink(this);
		}

		public SourceStatusType SourceStatus => StoredEnergy > 0
												? SourceStatusType.Passive
												: SourceStatusType.Disabled;
        public float SourcePriority => Props.sourcePriority;
        public float DesiredSourceRatePer1000Ticks => Props.desiredTransferRatePer1000Ticks;
        public float CurrentMaxSourcableEnergy => StoredEnergy;

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

		public float StoragePriority => Props.storagePriority;

		public float StoredEnergy => this.storedEnergy;

		public float StorageCapacity => Props.storageCapacity;

		public StorageLevelTag StorageLevel => this.GetDefaultStorageLevel();

		public SinkStatusType SinkStatus => StoredEnergy <= StorageCapacity - 0.000001f
												? SinkStatusType.Passive
												: SinkStatusType.Disabled;

		public float SinkPriority => Props.sinkPriority;

		public float DesiredSinkRatePer1000Ticks => Props.desiredTransferRatePer1000Ticks;

		public float CurrentMaxSinkableEnergy => StorageCapacity - StoredEnergy;

		public void SourceEnergy(float amount)
        {
            storedEnergy -= amount;
            storedEnergy = Mathf.Max(storedEnergy, 0);

			this.wasLastSourcedFromNotSinked = true;
			UpdatePercentageChangeAndLastTick(amount);
        }

		public void UpdatePercentageChangeAndLastTick(float amount)
		{
            int currentTick = Find.TickManager.TicksGame;
            if(currentTick != lastTickWorked)
                this.hourlyPercentageChange = (amount / StorageCapacity) * 2500f/ (float)(currentTick - lastTickWorked);
            lastTickWorked = currentTick;
		}
        
        public override void CompExposeData()
        {
            this.ForceRegisterReferenceable();
            Scribe_Values.Look<int>(ref this.lastTickWorked, "LastTickWorked");
			Scribe_Values.Look<float>(ref this.storedEnergy, "StoredEnergy");
        }

        public string GetUniqueLoadID() => parent.GetUniqueLoadID() + "_energySource";

		public void SetEnergyDirect(float amount) => this.storedEnergy = Mathf.Clamp(amount, 0, StorageCapacity);

		public void SinkEnergy(float amount)
        {
            storedEnergy += amount;
            storedEnergy = Mathf.Min(storedEnergy, StorageCapacity);
            
            this.wasLastSourcedFromNotSinked = false;
            UpdatePercentageChangeAndLastTick(amount);
        }

		public override void LoadSettingsFromThingComp(ThingComp_AndroidImplant implant) =>
			SetEnergyDirect((implant as IEnergyStorage).StoredEnergy);

		public void SinkAttached(EnergySystem system) { }

		public void SinkDetached(EnergySystem system) { }

		public void SourceAttached(EnergySystem system) { }

		public void SourceDetached(EnergySystem system) { }
	}
}
