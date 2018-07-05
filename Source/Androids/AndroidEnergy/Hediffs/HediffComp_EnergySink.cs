using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffComp_EnergySink : HediffComp_AndroidImplant, IEnergySink
    {
		public HediffProperties_EnergySink Props => (HediffProperties_EnergySink)this.props;

		public EnergySystem EnergySystem => this.Pawn.TryGetComp<EnergySystem>();

		int lastTickWorked;
		float energyConsumedPerHour;
    
        public HediffComp_EnergySink()
        {
			lastTickWorked = Find.TickManager.TicksGame - 1;
        }
        
        public override string CompTipStringExtra {
            get {
                return "AT.HediffComp.EnergyConsumedPerHour.TipString"
                            .Translate(energyConsumedPerHour);
            }
        }

		public override void CompPostPostAdd(DamageInfo? dinfo) =>
            EnergySystem.AttachSink(this);

		public override void CompPostPostRemoved() => EnergySystem.DetachSink(this);

		public SinkStatusType SinkStatus => Props.activeSink
												? SinkStatusType.Active
												: SinkStatusType.Passive;
		public float SinkPriority => Props.sinkPriority;
		public float DesiredSinkRatePer1000Ticks => Props.desiredSinkRatePer1000Ticks;
		public float CurrentMaxSinkableEnergy => (float)Math.Min(Find.TickManager.TicksGame - lastTickWorked
					, EnergyConstants.maxTicksForNotStorageToBuildEnergy) * DesiredSinkRatePer1000Ticks / 1000f;
		public void SinkEnergy(float amount) 
        {
            int currentTick = Find.TickManager.TicksGame;
            if(currentTick != lastTickWorked)
                energyConsumedPerHour = amount * 2500f / (float)(currentTick - lastTickWorked);
            lastTickWorked = currentTick;
        }
        
        public override void CompExposeData()
        {
			this.ForceRegisterReferenceable();
			Scribe_Values.Look<int>(ref this.lastTickWorked, "LastTickWorked");
        }

		public string GetUniqueLoadID() => parent.GetUniqueLoadID() + "_energySink";

		public override void LoadSettingsFromThingComp(ThingComp_AndroidImplant implant) { }

		public void SinkAttached(EnergySystem system) { }

	    public void SinkDetached(EnergySystem system) { }	
	}
}
