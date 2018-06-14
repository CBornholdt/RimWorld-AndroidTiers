using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
	public enum EnergyNeedCategory : byte { Acceptable, Low, Critical, Empty };

    public class Need_Energy : Need
    {
		public Need_Energy() => this.threshPercents = new List<float>(3);

		public CompProperties_NeedsEnergy Props => this.pawn.TryGetComp<CompNeedsEnergy>()?.Props;
        
        public EnergyNeedCategory LowEnergyNeed {
			get {
				switch(this.CurLevel) {
					case var testLevel when testLevel <= 0.00001:
						return EnergyNeedCategory.Empty;
					case var testLevel when testLevel <= Props.criticallyLowLevelThreshPercent:
						return EnergyNeedCategory.Critical;
					case var testLevel when testLevel <= Props.lowLevelThreshPercent:
						return EnergyNeedCategory.Low;
				}
				return EnergyNeedCategory.Acceptable;
			}
		}

		public float RechargeThreshold => Props.lowLevelThreshPercent + EnergyConstants.AdditionalRechargeThresholdFromLow;
                             
		public Need_Energy(Pawn pawn) : base(pawn)
		{
			this.threshPercents = new List<float>(3);
		}

		override public void SetInitialLevel() => CurLevel = 1f;

		override public float MaxLevel => 1f;

		override public void DrawOnGUI(Rect rect, int maxThresholdMarkers = int.MaxValue, float customMargin = -1F, bool drawArrows = true, bool doTooltip = true)
		{
			this.threshPercents.Clear();
			this.threshPercents.Add(Props.criticallyLowLevelThreshPercent);
			this.threshPercents.Add(Props.lowLevelThreshPercent);
			base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
		}

		override public void NeedInterval()
        {
			this.CurLevel -=  150f * Props.ValueLossPerTick;    //150 ticks per NeedsInterval
			AdjustLowEnergyHediffs();         
		}

		public void AddPower(float amount) => this.CurLevel += amount * MaxLevel / Props.powerForFullEnergy;

		public void AdjustLowEnergyHediffs()
		{
			EnergyNeedCategory eNeeds = LowEnergyNeed;
			if(Props.emptyLevelHediff != null && eNeeds != EnergyNeedCategory.Empty
				&& this.pawn.health.hediffSet.HasHediff(Props.emptyLevelHediff))
				foreach(var hediff in this.pawn.health.hediffSet.hediffs.Where(hd => hd.def == Props.emptyLevelHediff))
					this.pawn.health.RemoveHediff(hediff);

			if(Props.criticallyLowLevelHediff != null && eNeeds != EnergyNeedCategory.Critical
				&& this.pawn.health.hediffSet.HasHediff(Props.criticallyLowLevelHediff))
				foreach(var hediff in this.pawn.health.hediffSet.hediffs.Where(hd => hd.def == Props.criticallyLowLevelHediff))
					this.pawn.health.RemoveHediff(hediff);

			if(Props.emptyLevelHediff != null && eNeeds != EnergyNeedCategory.Low
				&& this.pawn.health.hediffSet.HasHediff(Props.lowLevelHediff))
				foreach(var hediff in this.pawn.health.hediffSet.hediffs.Where(hd => hd.def == Props.lowLevelHediff))
					this.pawn.health.RemoveHediff(hediff);
		}
        
        
	}
}
