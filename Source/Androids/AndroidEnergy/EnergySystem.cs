using System;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Harmony;

namespace MOARANDROIDS
{
	public class EnergySystem : ThingComp, IThingHolder, IEnergyStorage
	{
		List<IEnergySink> attachedSinksSorted;
		List<IEnergySource> attachedSourcesSorted;
		ThingOwner<ThingWithComps> installedComps;

		BaseEnergyConsumption baseEnergyConsumption;

		public List<IEnergySink> AttachedSinks => attachedSinksSorted;
		public List<IEnergySource> AttachedSources => attachedSourcesSorted;
		public IEnumerable<EnergySystemComp> InstalledComps => installedComps.InnerListForReading
				.Select(thing => (EnergySystemComp)thing.AllComps.First(comp => comp is EnergySystemComp));

		private int lastTickWorked;

		public CompProperties_EnergySystem Props => (CompProperties_EnergySystem)this.props;

		public EnergySystem() { }

		public override void PostExposeData()
        {
			Scribe_Collections.Look<IEnergySink>(ref this.attachedSinksSorted, "AttachedSinksSorted", LookMode.Reference);
            Scribe_Collections.Look<IEnergySource>(ref this.attachedSourcesSorted, "AttachedSourcesSorted", LookMode.Reference);
			Scribe_Deep.Look<ThingOwner<ThingWithComps>>(ref this.installedComps, "InstalledComps");
			Scribe_Deep.Look<BaseEnergyConsumption>(ref this.baseEnergyConsumption, "BaseEnergyComsumption");

			if(Scribe.mode == LoadSaveMode.LoadingVars)
				Traverse.Create(Scribe.loader.crossRefs).Field("loadedObjectDirectory").Method("RegisterLoaded", new object[1] { this });  
        }
        
		//ILoadReferenceable stuff
		public string GetUniqueLoadID() => this.parent.GetUniqueLoadID() + "_EnergySystem";

        //IThingHolder stuff
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			foreach(var childHolder in installedComps.InnerListForReading
                                        .Where(comp => comp is IThingHolder).Cast<IThingHolder>())
				outChildren.Add(childHolder);
		}

		public ThingOwner GetDirectlyHeldThings() => installedComps;
        
        //IEnergyStorage stuff ... writing without Linq for speed
        public float StoredEnergy {
			get {
				float storedEnergy = 0;
				for(int i = 0; i < attachedSourcesSorted.Count; i++)
					if(attachedSourcesSorted[i] is IEnergyStorage storage
                        && storage.ActiveStorageOrFull())
						storedEnergy += storage.StoredEnergy;
				return storedEnergy;
			}
		}
        
        public StorageStatusType StorageStatus {
			get {
				if(this.StoredEnergy <= 0) {
					if(this.StorageCapacity > 0)
						return StorageStatusType.Empty;
					return StorageStatusType.NotActive;
				}
				if(this.StoredEnergy >= this.StorageCapacity)
					return StorageStatusType.Full;
				return StorageStatusType.Active;
			}
		}
        
        public float StorageCapacity {
			get {
				float storageCapacity = 0;
				for(int i = 0; i < attachedSinksSorted.Count; i++)
					if(attachedSinksSorted[i] is IEnergyStorage storage
                        && storage.ActiveStorageOrEmpty())
						storageCapacity += storage.StorageCapacity;
				return storageCapacity;
			}
		}

		public float StoragePriority => 1f;     //TODO need to think about this 
        
        //IEnergySink stuff
        public SinkStatusType SinkStatus {
			get {
				if(this.ActiveStorageOrEmpty())
					return SinkStatusType.Active;
				for(int i = 0; i < attachedSinksSorted.Count; i++)
					if(attachedSinksSorted[i].SinkStatus == SinkStatusType.Active)
						return SinkStatusType.Active;
				return SinkStatusType.NotActive;
			}
		}
        
        public float DesiredSinkRatePer1000Ticks {
			get {
				float desiredSinkRate = 0;
				for(int i = 0; i < attachedSinksSorted.Count; i++)
					if(attachedSinksSorted[i].SinkStatus == SinkStatusType.Active)
						desiredSinkRate += attachedSinksSorted[i].DesiredSinkRatePer1000Ticks;
				return desiredSinkRate;
			}
		}

		public float TrySinkEnergy(float amount)
		{
			float amountSunk = 0;
			for(int i = 0; i < attachedSinksSorted.Count; i++)
				if(attachedSinksSorted[i].SinkStatus == SinkStatusType.Active) {
					float amountRemaining = amount - amountSunk;
					amountSunk += attachedSinksSorted[i].TrySinkEnergy(amountRemaining);
					if(Mathf.Approximately(amount, amountSunk))
						return amount;
				}
			return amountSunk;
		}

		public float SinkPriority => 1f;    //TODO need to think about this
        
        //IEnergySource stuff
        public SourceStatusType SourceStatus {
			get {
				if(this.ActiveStorageOrFull())
					return SourceStatusType.Active;
				for(int i = 0; i < attachedSourcesSorted.Count; i++)
					if(attachedSourcesSorted[i].SourceStatus == SourceStatusType.Active)
						return SourceStatusType.Active;
				return SourceStatusType.NotActive;
			}
		}
        
        public float DesiredSourceRatePer1000Ticks {
			get {
				float desiredSourceRate = 0;
				for(int i = 0; i < attachedSourcesSorted.Count; i++)
					if(attachedSourcesSorted[i].SourceStatus == SourceStatusType.Active)
						desiredSourceRate += attachedSourcesSorted[i].DesiredSourceRatePer1000Ticks;
				return desiredSourceRate;
			}
		}

		public float TrySourceEnergy(float amount)
		{
			float amountSourced = 0;
			for(int i = 0; i < attachedSourcesSorted.Count; i++)
				if(attachedSourcesSorted[i].SourceStatus == SourceStatusType.Active) {
					float amountRemaining = amount - amountSourced;
					amountSourced += attachedSourcesSorted[i].TrySourceEnergy(amountRemaining);
					if(Mathf.Approximately(amount, amountSourced))
						return amount;
				}
			return amountSourced;
		}

		public float SourcePriority => 1f;

		//ThingComp stuff
		override public void CompTick()
		{
			if(!(this.parent as Pawn).IsHashIntervalTick(100))
				return;
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
            this.lastTickWorked = Find.TickManager.TicksGame - 1;              
		}

		private class BaseEnergyConsumption : IEnergySink, IExposable
		{
			EnergySystem parent;
			int lastTickWorked;
            
            public BaseEnergyConsumption() => this.lastTickWorked = Find.TickManager.TicksGame - 1;

			public BaseEnergyConsumption(EnergySystem parent) : this() => this.parent = parent;

			public void ExposeData()
			{
				Scribe_References.Look<EnergySystem>(ref this.parent, "Parent");
			}

			public string GetUniqueLoadID() => parent.GetUniqueLoadID() + "_BEC";

			public SinkStatusType SinkStatus => SinkStatusType.Active;

			public float SinkPriority => 100f;

			public float DesiredSinkRatePer1000Ticks => parent.Props.baseEnergyConsumptionPer1000Ticks / 1000f;

			public float TrySinkEnergy(float amount)
			{
				float desiredSinkAmount = DesiredSinkRatePer1000Ticks * (float)Math.Min((Find.TickManager.TicksGame - lastTickWorked), 200);
				this.lastTickWorked = Find.TickManager.TicksGame;
				return Mathf.Min(amount, desiredSinkAmount);
			}
		}
	}
}
