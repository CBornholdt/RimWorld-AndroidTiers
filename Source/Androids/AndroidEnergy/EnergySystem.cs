using System;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Harmony;
using RimWorld;

namespace MOARANDROIDS
{
	public class EnergySystem : ThingComp, IThingHolder, IEnergyStorage
	{
		List<AttachedSink> attachedSinksSorted;
		List<AttachedSource> attachedSourcesSorted;
		ThingOwner<ThingWithComps> installedComps;

		BaseEnergyConsumption baseEnergyConsumption;

		public IEnumerable<IEnergySink> AttachedSinks => attachedSinksSorted.Select(aSink => aSink.sink);
		public IEnumerable<IEnergySource> AttachedSources => attachedSourcesSorted.Select(aSource => aSource.source);
		public IEnumerable<IEnergyStorage> AttachedStorages => AttachedSinks.OfType<IEnergyStorage>()
										.Concat(AttachedSources.OfType<IEnergyStorage>())
                                        .Cast<IEnergyStorage>()
                                        .Distinct();
           
		public IEnumerable<EnergySystemComp> InstalledComps => installedComps.InnerListForReading
			.Select(thing => (EnergySystemComp)thing.TryGetComp<EnergySystemComp>());

		private int lastTickWorked;

		public Pawn Owner => this.parent as Pawn;

		public EnergySystem()
		{
			this.attachedSinksSorted = new List<AttachedSink>(4);
			this.attachedSourcesSorted = new List<AttachedSource>(4);
			this.installedComps = new ThingOwner<ThingWithComps>(this);
			this.lastTickWorked = Find.TickManager.TicksGame - 1;
		}

		public void AttachSink(IEnergySink sink, DisconnectWhen when = new DisconnectWhen())
		{
			attachedSinksSorted.Add(new AttachedSink(sink, when));
			attachedSinksSorted.SortByDescending<AttachedSink, float>(s => s.sink.SinkPriority);
		}

		public void AttachSource(IEnergySource source, DisconnectWhen when = new DisconnectWhen())
		{
            attachedSourcesSorted.Add(new AttachedSource(source, when));
            attachedSourcesSorted.SortByDescending<AttachedSource, float>(s => s.source.SourcePriority);
		}

		public void DetachSink(IEnergySink sink) => attachedSinksSorted.RemoveAll(aSink => aSink.sink == sink);

		public void DetachSource(IEnergySource source) => attachedSourcesSorted.RemoveAll(aSource => aSource.source == source);

		public void UpdateDisconnections()
		{
			for(int i = attachedSourcesSorted.Count; --i >= 0;)
				if(attachedSourcesSorted[i].disconnectWhen.ShouldDisconnectSystemFrom(this, attachedSourcesSorted[i].source))
					attachedSourcesSorted.RemoveAt(i);
			for(int i = attachedSinksSorted.Count; --i >= 0;)
				if(attachedSinksSorted[i].disconnectWhen.ShouldDisconnectSystemFrom(this, attachedSinksSorted[i].sink))
					attachedSinksSorted.RemoveAt(i);
		}  
                

		public void InstallEnergySystemComp(EnergySystemComp newComp)
		{
			if(newComp.parent.Spawned)
                newComp.parent.DeSpawn();
			installedComps.TryAdd(newComp.parent, canMergeWithExistingStacks: false);
			if(newComp is IEnergySink sink)
				AttachSink(sink);
			if(newComp is IEnergySource source)
				AttachSource(source);
			newComp.Installed(this);
		}

		public void RemoveEnergySystemComp(EnergySystemComp comp)
		{
			if(!installedComps.Contains(comp.parent))
				return;

			comp.Removed();
			if(comp is IEnergySink sink)
				DetachSink(sink);
			if(comp is IEnergySource source)
				DetachSource(source);
			installedComps.TryDrop(comp.parent, this.parent.Position, this.parent.Map
			   , ThingPlaceMode.Near, count: 1, resultingThing: out Thing resultingThing); 
		}

		public int LastTickProcessed => this.lastTickWorked;

		public void SetEnergyDirect(float amount)
		{
			float ePercent = Mathf.Min(amount / this.StorageCapacity, 1f);
			foreach(var storage in AttachedStorages)
				storage.SetEnergyDirect(ePercent * storage.StorageCapacity);
		}

        //ThingComp or ThingComp-like stuff
        public CompProperties_EnergySystem Props => (CompProperties_EnergySystem)this.props;
        
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
			this.ForceRegisterReferenceable();
            
            Scribe_Collections.Look<AttachedSink>(ref this.attachedSinksSorted, "AttachedSinksSorted", LookMode.Deep);
            Scribe_Collections.Look<AttachedSource>(ref this.attachedSourcesSorted, "AttachedSourcesSorted", LookMode.Deep);
            Scribe_Deep.Look<ThingOwner<ThingWithComps>>(ref this.installedComps, "InstalledComps");
			Scribe_Values.Look<int>(ref this.lastTickWorked, "LastTickWorked");
        }

		void PostPostMake()
		{
			attachedSinksSorted.Add(new AttachedSink(this.baseEnergyConsumption));

			if(this.Owner.health == null)
				PawnComponentsUtility.CreateInitialComponents(this.Owner);

			Hediff batteryHediff;
			if(Props.initialBattery != null) {
				batteryHediff = HediffMaker.MakeHediff(Props.initialBattery, this.Owner, null);
				var energyStorage = batteryHediff.TryGetComp<HediffComp_EnergyStorage>();
				energyStorage.SetEnergyDirect(energyStorage.StorageCapacity);
			}
			else
				batteryHediff = HediffMaker.MakeHediff(EnergyHediffs.AT_NotPresent, this.Owner, null);
			this.Owner.health.AddHediff(batteryHediff
									, Owner.RaceProps.body.AllParts.First(part => part.def == AndroidParts.MBattery));
            this.Owner.health.AddHediff(Props.initialReactor ?? EnergyHediffs.AT_NotPresent
                                    , Owner.RaceProps.body.AllParts.First(part => part.def == AndroidParts.MReactor));
            this.Owner.health.AddHediff(Props.initialConduit ?? EnergyHediffs.AT_NotPresent
                                    , Owner.RaceProps.body.AllParts.First(part => part.def == AndroidParts.MConduit));
            
			Log.Message($"PostPostMake {StoredEnergy} {StorageCapacity}");
		}
        
        override public void CompTick()
        {
            if(!(this.parent as Pawn).IsHashIntervalTick(EnergyConstants.TicksPerSystemUpdate))
                return;

			UpdateDisconnections();
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
					if(attachedSourcesSorted[i].source is IEnergyStorage storage
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
					if(attachedSinksSorted[i].sink is IEnergyStorage storage
                        && storage.StorageStatus != StorageStatusType.NotActive)
						storageCapacity += storage.StorageCapacity;
				return storageCapacity;
			}
		}

		public float StoragePriority => 1f;     //TODO need to think about this 

		//TODO implement stat based on xml
		public StorageLevelTag StorageLevel => this.GetDefaultStorageLevel();
        
        //IEnergySink stuff
        public SinkStatusType SinkStatus {
			get {
				if(this.ActiveStorageOrEmpty())
					return SinkStatusType.Active;
				for(int i = 0; i < attachedSinksSorted.Count; i++)
					if(attachedSinksSorted[i].sink.SinkStatus == SinkStatusType.Active)
						return SinkStatusType.Active;
				return SinkStatusType.Disabled;
			}
		}
        
        public float DesiredSinkRatePer1000Ticks {
			get {
				float desiredSinkRate = 0;
				for(int i = 0; i < attachedSinksSorted.Count; i++)
					if(attachedSinksSorted[i].sink.SinkStatus == SinkStatusType.Active)
						desiredSinkRate += attachedSinksSorted[i].sink.DesiredSinkRatePer1000Ticks;
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
					if(attachedSourcesSorted[i].source.SourceStatus == SourceStatusType.Active)
						return SourceStatusType.Active;
				return SourceStatusType.Disabled;
			}
		}

		public float CurrentMaxSourcableEnergy => StoredEnergy;
        
        public float DesiredSourceRatePer1000Ticks {
			get {
				float desiredSourceRate = 0;
				for(int i = 0; i < attachedSourcesSorted.Count; i++)
					if(attachedSourcesSorted[i].source.SourceStatus == SourceStatusType.Active)
						desiredSourceRate += attachedSourcesSorted[i].source.DesiredSourceRatePer1000Ticks;
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
			IEnumerator<IEnergySink> sinkEnumerator = AttachedSinks.GetEnumerator();
			IEnumerator<IEnergySource> sourceEnumerator = AttachedSources.GetEnumerator();
			IEnergySource source = null;
			IEnergySink sink = null;
			float maxDrawOnSource = 0;
			float amountDrawnOnSource = 0;
			float maxPlaceInSink = 0;
			float amountPlacedInSink = 0;

			this.lastTickWorked = Find.TickManager.TicksGame;
            
            Action applyEnergyToSink = () => {
				if(amountPlacedInSink > 0)
					sink?.SinkEnergy(amountPlacedInSink);
			};
			Action applyEnergyToSource = () => {
				if(amountDrawnOnSource > 0)
					source?.SourceEnergy(amountDrawnOnSource);
			};
            
			Func<bool> tryAdvanceSourceEnumerator = () => {
				if(!sourceEnumerator.MoveNext()) {
					applyEnergyToSink();    //Ran out of sources, process last sink
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
					//Sinks cannot pull on themselves if also a source, nor can two passive types (passive = 1, disabled = 0)             
                    //TODO I think there will be an issue if an active sink follows a passive one, is that ok?
					if(source as IEnergySink != sink && (byte)sink.SinkStatus + (byte)source.SourceStatus > 2) {
						float transferAmount = Mathf.Min(maxPlaceInSink - amountPlacedInSink
													, maxDrawOnSource - amountDrawnOnSource);
						amountPlacedInSink += transferAmount;
						amountDrawnOnSource += transferAmount;                            
					}
					if(amountPlacedInSink >= (maxPlaceInSink - 0.0000001))  //Epsilon as a precaution
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

		private class AttachedSink : IExposable
		{
			public IEnergySink sink;
			public DisconnectWhen disconnectWhen;

			public AttachedSink(IEnergySink sink, DisconnectWhen when = new DisconnectWhen())
			{
				this.sink = sink;
				this.disconnectWhen = when;
			}

			public void ExposeData()
			{
				Scribe_References.Look<IEnergySink>(ref this.sink, "Sink");
				Scribe_Values.Look<DisconnectWhen>(ref this.disconnectWhen, "DisconnectWhen");
			}
		}
        
        private class AttachedSource : IExposable
        {
            public IEnergySource source;
            public DisconnectWhen disconnectWhen;
            
            public AttachedSource(IEnergySource source, DisconnectWhen when)
            {
                this.source = source;
                this.disconnectWhen = when;
            }

            public void ExposeData()
            {
                Scribe_References.Look<IEnergySource>(ref this.source, "Source");
                Scribe_Values.Look<DisconnectWhen>(ref this.disconnectWhen, "DisconnectWhen");
            }
        }
		
		private class BaseEnergyConsumption : IEnergySink
		{
			EnergySystem parent;
			int lastTickWorked;
            
            public BaseEnergyConsumption() => this.lastTickWorked = Find.TickManager.TicksGame - 1;

			public BaseEnergyConsumption(EnergySystem parent) : this()
			{
				this.parent = parent;

				this.ForceRegisterReferenceable();
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
