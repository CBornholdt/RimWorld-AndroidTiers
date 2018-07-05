using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class AT_ModSettings : ModSettings
    {
		public EnergySearchSettings energySearchSettings = new EnergySearchSettings();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<EnergySearchSettings>(ref this.energySearchSettings, "EnergySearchSettings");
		}
	}
}
