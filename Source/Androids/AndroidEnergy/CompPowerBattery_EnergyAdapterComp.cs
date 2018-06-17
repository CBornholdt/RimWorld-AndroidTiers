using System;
using Verse;
using RimWorld;
using Harmony;
using UnityEngine;

namespace MOARANDROIDS
{
	public class CompPowerBattery_EnergyAdapterComp : CompPowerBattery, IEnergySource
    {
		public new CompPowerBattery_EnergyAdaptorProps Props =>
				(CompPowerBattery_EnergyAdaptorProps)this.props;

		public SourceStatusType SourceStatus => (this.StoredEnergy > 0)
						? SourceStatusType.Active : SourceStatusType.Disabled;

		public float SourcePriority => Props.sourcePriority;

		public float DesiredSourceRatePer1000Ticks => Props.desiredTranferRatePer1000Ticks;

		public float CurrentMaxSourcableEnergy => base.StoredEnergy / Props.powerPerUnitEnergy;

		public void SourceEnergy(float amount)
		{
			float powerLost = amount * Props.powerPerUnitEnergy;
			this.DrawPower(Mathf.Min(powerLost, base.StoredEnergy));
		}                         

		public override void PostExposeData()
		{
			base.PostExposeData();
			this.ForceRegisterReferenceable(); 
		}

		public string GetUniqueLoadID() => this.parent.GetUniqueLoadID() + "_EAC";  
    }
}
