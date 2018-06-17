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
			.Select(thing => (EnergySystemComp)thing.TryGetComp<EnergySystemComp>());

		private int lastTickWorked;

		public CompProperties_EnergySystem Props => (CompProperties_EnergySystem)this.props;

		public EnergySystem()
		{
			this.attachedSinksSorted = new List<IEnergySink>(4);
			this.attachedSourcesSorted = new List<IEnergySource>(4);
			this.installedComps = new ThingOwner<ThingWithComps>(this);
		}

		public void InstallEnergySystemComp(EnergySystemComp newComp)
		{
			installedComps.TryAdd(newComp.parent, canMergeWithExistingStacks: false);
			if(newComp is IEnergySink sink) {
				attachedSinksSorted.Add(sink);
				attachedSinksSorted.SortByDescending<IEnergySink, float>(s => s.SinkPriority);
			}
            if(newComp is IEnergySource source) {
                attachedSourcesSorted.Add(source);
                attachedSourcesSorted.SortByDescending<IEnergySource, float>(s => s.SourcePriority);
            }
			newComp.Installed(this);
		}

        //ThingComp or ThingComp-like stuff
		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
            this.baseEnergyConsumption = new BaseEnergyConsumption(this);
			if(Scribe.mode == LoadSaveMode.Inactive)
				PostPostMake();
            this.lastTickWorked = Find.TickManager.TicksGame - 1;  
		}

		public override void PostExposeData()
        {        
			if(Scribe.mode == LoadSaveMode.LoadingVars) 
                Traverse.Create(Scribe.loader.crossRefs).Field("loadedObjectDirectory")
                    .Method("RegisterLoaded", new object[1] { this }).GetValue();
            
            Scribe_Collections.Look<IEnergySink>(ref this.attachedSinksSorted, "AttachedSinksSorted", LookMode.Reference);
            Scribe_Collections.Look<IEnergySource>(ref this.attachedSourcesSorted, "AttachedSourcesSorted", LookMode.Reference);
            Scribe_Deep.Look<ThingOwner<ThingWithComps>>(ref this.installedComps, "InstalledComps");    
        }

		void PostPostMake()
		{
			attachedSinksSorted.Add(this.baseEnergyConsumption);
			foreach(var compDef in this.Props.initialComponentTypes ?? Enumerable.Empty<ThingDef>())
				InstallEnergySystemComp(ThingMaker.MakeThing(compDef).TryGetComp<EnergySystemComp>());
		}
        
        override public void CompTick()
        {
            if(!(this.parent as Pawn).IsHashIntervalTick(EnergyConstants.TicksPerSystemUpdate))
                return;

            EnergySystemTick();
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
				return SinkStatusType.Disabled;
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

		public float CurrentMaxSinkableEnergy => StorageCapacity - StoredEnergy;

		public void SinkEnergy(float amount)
		{
			
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
				return SourceStatusType.Disabled;
			}
		}

		public float CurrentMaxSourcableEnergy => StoredEnergy;
        
        public float DesiredSourceRatePer1000Ticks {
			get {
				float desiredSourceRate = 0;
				for(int i = 0; i < attachedSourcesSorted.Count; i++)
					if(attachedSourcesSorted[i].SourceStatus == SourceStatusType.Active)
						desiredSourceRate += attachedSourcesSorted[i].DesiredSourceRatePer1000Ticks;
				return desiredSourceRate;
			}
		}

		public void SourceEnergy(float amount)
		{
		    	
		}

		public float SourcePriority => 1f;		

		//Energy System internals
		public void EnergySystemTick()
		{
			IEnumerator<IEnergySink> sinkEnumerator = attachedSinksSorted.GetEnumerator();
			IEnumerator<IEnergySource> sourceEnumerator = attachedSourcesSorted.GetEnumerator();
			IEnergySource source = null;
			IEnergySink sink = null;
			float maxDrawOnSource = 0;
			float amountDrawnOnSource = 0;
			float maxPlaceInSink = 0;
			float amountPlacedInSink = 0;
            
            Action applyEnergyToSink = () => {
				if(sink != null && amountPlacedInSink > 0)
					sink.SinkEnergy(amountPlacedInSink);
			};
			Action applyEnergyToSource = () => {
				if(source != null && amountDrawnOnSource > 0)
					source.SourceEnergy(amountDrawnOnSource);
			};
            
			Func<bool> tryAdvanceSourceEnumerator = () => {
				if(!sourceEnumerator.MoveNext()) {
					applyEnergyToSink();
					return false;
				}	
				source = sourceEnumerator.Current;
				maxDrawOnSource = Mathf.Min(source.CurrentMaxSourcableEnergy
										, source.DesiredSourceRatePer1000Ticks 
                                            * EnergyConstants.TicksPerSystemUpdate / 1000f);
				amountDrawnOnSource = 0;
				return true;
			};

			if(!tryAdvanceSourceEnumerator())
				return;
			while(sinkEnumerator.MoveNext()) {
				sink = sinkEnumerator.Current;
				maxPlaceInSink = Mathf.Min(sink.CurrentMaxSinkableEnergy
												, sink.DesiredSinkRatePer1000Ticks 
                                                    * EnergyConstants.TicksPerSystemUpdate / 1000f);
				amountPlacedInSink = 0;
				while(true) {
					//Sinks cannot pull on themselves if also a source, nor can two passive types (passive = 1)
                    
                    //TODO I think there will be an issue if an active sink follows a passive one, is that ok?
					if(source as IEnergySink != sink && (byte)sink.SinkStatus + (byte)source.SourceStatus > 2) {
						float transferAmount = Mathf.Min(maxPlaceInSink - amountPlacedInSink
													, maxDrawOnSource - amountDrawnOnSource);
						amountPlacedInSink += transferAmount;
						amountDrawnOnSource += transferAmount;                            
					}
					if(amountPlacedInSink >= maxPlaceInSink)
						break;
					applyEnergyToSource();
					if(!tryAdvanceSourceEnumerator())
						return;
				}
				applyEnergyToSink();
			}
			//Ran out of sinks first, process last source
			applyEnergyToSource();                                                                                          
		}
		
		private class BaseEnergyConsumption : IEnergySink
		{
			EnergySystem parent;
			int lastTickWorked;
            
            public BaseEnergyConsumption() => this.lastTickWorked = Find.TickManager.TicksGame - 1;

			public BaseEnergyConsumption(EnergySystem parent) : this()
			{
				this.parent = parent;
                
                if(Scribe.mode == LoadSaveMode.LoadingVars) 
                Traverse.Create(Scribe.loader.crossRefs).Field("loadedObjectDirectory")
                    .Method("RegisterLoaded", new object[1] { this }).GetValue();
			}

			public string GetUniqueLoadID() => parent.GetUniqueLoadID() + "_BEC";

			public SinkStatusType SinkStatus => SinkStatusType.Active;

			public float SinkPriority => 100f;

			public float DesiredSinkRatePer1000Ticks => parent.Props.baseEnergyConsumptionPer1000Ticks;

			public float CurrentMaxSinkableEnergy => (float)(Find.TickManager.TicksGame - lastTickWorked) *
                DesiredSinkRatePer1000Ticks / 1000f;

			public void SinkEnergy(float amount) => this.lastTickWorked = Find.TickManager.TicksGame;
		}
	}
}
