using System;
using Verse;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
    public class Comp_PassiveEnergySource : ThingComp
    {
		int lastTickDrawnFrom;
    
		public CompProperties_PassiveEnergySource Props => (CompProperties_PassiveEnergySource)this.props;

        public float TryDrawEnergy(float amount)
        {
			float amountAvailable = (float)Math.Max(Find.TickManager.TicksGame - lastTickDrawnFrom
						, Props.maximumTickInterval) * Props.energyPer1000Ticks / 1000f;
			lastTickDrawnFrom = Find.TickManager.TicksGame;
			return Mathf.Min(amountAvailable, amount);
        }

		public float NaturalDrawRate => Props.energyPer1000Ticks / 1000f;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			this.lastTickDrawnFrom = Find.TickManager.TicksGame - 1;
		}
	}
}
