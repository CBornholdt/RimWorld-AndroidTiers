using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffComp_EnergySource : HediffComp_AndroidImplant, IEnergySource
    {
        public HediffProperties_EnergySource Props => (HediffProperties_EnergySource)this.props;

        public EnergySystem EnergySystem => this.Pawn.TryGetComp<EnergySystem>();

        int lastTickWorked;
		float energyProvidedPerHour;
    
        public HediffComp_EnergySource()
        {
			lastTickWorked = Find.TickManager.TicksGame - 1;
        }

		public override string CompTipStringExtra {
			get {
				return "AT.HediffComp.EnergyProvidedPerHour.TipString"
							.Translate(energyProvidedPerHour);
			}
		}

		public override void CompPostPostAdd(DamageInfo? dinfo) =>
            EnergySystem.AttachSource(this);

        public override void CompPostPostRemoved() => EnergySystem.DetachSource(this);

        public SourceStatusType SourceStatus => Props.activeSource
                                                ? SourceStatusType.Active
                                                : SourceStatusType.Passive;
        public float SourcePriority => Props.sourcePriority;
        public float DesiredSourceRatePer1000Ticks => Props.desiredSourceRatePer1000Ticks;
        public float CurrentMaxSourcableEnergy => (float)Math.Min(Find.TickManager.TicksGame - lastTickWorked
                    , EnergyConstants.maxTicksForNotStorageToBuildEnergy) * DesiredSourceRatePer1000Ticks / 1000f;

		public void SourceEnergy(float amount)
		{
			int currentTick = Find.TickManager.TicksGame;
			if(currentTick != lastTickWorked)
				energyProvidedPerHour = amount * 2500f / (float)(currentTick - lastTickWorked);
			lastTickWorked = currentTick;
		}
        
        public override void CompExposeData()
        {
            this.ForceRegisterReferenceable();
            Scribe_Values.Look<int>(ref this.lastTickWorked, "LastTickWorked", Find.TickManager.TicksGame - 1);
        }

        public string GetUniqueLoadID() => parent.GetUniqueLoadID() + "_energySource";

		public override void LoadStateFromThingComp(ThingComp_AndroidImplant implant) { }

		public void SourceAttached(EnergySystem system) { }

		public void SourceDetached(EnergySystem system) { }		
	}
}
