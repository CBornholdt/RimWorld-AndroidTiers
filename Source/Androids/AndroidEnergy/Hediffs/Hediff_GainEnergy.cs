using System;
using Verse;
using RimWorld;


namespace MOARANDROIDS
{
	public class Hediff_GainEnergy : Hediff
	{
		public float totalEnergyGain;
		public int totalTickLength;
		public int ticksPassed;

		public Need_Energy EnergyNeed => this.pawn.needs.TryGetNeed<Need_Energy>();

		override public bool ShouldRemove => ticksPassed >= totalTickLength;

		override public void Heal(float amount) { }

		override public void Tick()
		{
			EnergyNeed.CurLevel += totalEnergyGain / (float)ticksPassed;
			ticksPassed++;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.totalEnergyGain, "TotalEnergyGain");
			Scribe_Values.Look<int>(ref this.totalTickLength, "TotalTickLength");
			Scribe_Values.Look<int>(ref this.ticksPassed, "TicksPassed");
		}
	}
}
