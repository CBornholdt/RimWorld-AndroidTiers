using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
	public enum EnergyNeedCategory : byte { None, Minor, Moderate, Major, Critical };

    public class Need_Energy : Need
    {
		public Need_Energy() => this.threshPercents = new List<float>(3);

		public CompProperties_NeedsEnergy Props => this.pawn.TryGetComp<CompNeedsEnergy>()?.Props;

		public EnergySystem EnergySystem => this.pawn.TryGetComp<EnergySystem>();
        
        public EnergyNeedCategory EnergyNeed {
			get {
				float max = MaxLevel;
				switch(this.CurLevel) {
					case var testLevel when testLevel > max * 0.95:
						return EnergyNeedCategory.None;
					case var testLevel when testLevel > max *
									(Props.lowLevelThreshPercent + EnergyConstants.AdditionalRechargeThresholdFromLow):
						return EnergyNeedCategory.Minor;
					case var testLevel when testLevel > max * Props.lowLevelThreshPercent:
						return EnergyNeedCategory.Moderate;
					case var testLevel when testLevel > max * Props.criticallyLowLevelThreshPercent:
						return EnergyNeedCategory.Major;
					default:
						return EnergyNeedCategory.Critical;
				}
			}
		}
                             
		public Need_Energy(Pawn pawn) : base(pawn)
		{
			this.threshPercents = new List<float>(3);
		}

		override public void SetInitialLevel()
		{
			if(this.EnergySystem == null)
				this.CurLevel = 1f;
		}

		override public float CurLevel {
			get => this.EnergySystem?.StoredEnergy ?? base.CurLevel;
			set {
				if(this.EnergySystem != null)
					Log.ErrorOnce("Attempted to set Energy need level but connected to an energy system that should be interacted with instead", 87);
				base.CurLevel = value;
			}
		}

		public void SetCurLevelPercentDirect(float percent)
		{
			if(EnergySystem == null)
				this.CurLevel = percent * this.MaxLevel;
			else
				this.EnergySystem.SetEnergyDirect(percent * EnergySystem.StorageCapacity);
		}

		override public float MaxLevel => this.EnergySystem?.StorageCapacity ?? 1f;

		override public void DrawOnGUI(Rect rect, int maxThresholdMarkers = int.MaxValue, float customMargin = -1F, bool drawArrows = true, bool doTooltip = true)
		{
            this.threshPercents.Clear();
			this.threshPercents.Add(Props.criticallyLowLevelThreshPercent);
			this.threshPercents.Add(Props.lowLevelThreshPercent);
			base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
		}

		override public void NeedInterval()
        {
            if(EnergySystem == null)
			    this.CurLevel -= 150f * Props.ValueLossPerTick;    //150 ticks per NeedsInterval
			AdjustLowEnergyHediffs();         
		}

		public void AddPower(float amount) => this.CurLevel += amount * MaxLevel / Props.powerForFullEnergy;

		public void AdjustLowEnergyHediffs()
		{
			EnergyNeedCategory eNeeds = EnergyNeed;
			if((Props.emptyLevelHediff != null && this.CurLevel < 0.00001f)
				&& this.pawn.health.hediffSet.HasHediff(Props.emptyLevelHediff))
				foreach(var hediff in this.pawn.health.hediffSet.hediffs.Where(hd => hd.def == Props.emptyLevelHediff))
					this.pawn.health.RemoveHediff(hediff);

			if(Props.criticallyLowLevelHediff != null && eNeeds == EnergyNeedCategory.Critical
				&& this.pawn.health.hediffSet.HasHediff(Props.criticallyLowLevelHediff))
				foreach(var hediff in this.pawn.health.hediffSet.hediffs.Where(hd => hd.def == Props.criticallyLowLevelHediff))
					this.pawn.health.RemoveHediff(hediff);

			if(Props.lowLevelHediff != null && eNeeds != EnergyNeedCategory.Major
				&& this.pawn.health.hediffSet.HasHediff(Props.lowLevelHediff))
				foreach(var hediff in this.pawn.health.hediffSet.hediffs.Where(hd => hd.def == Props.lowLevelHediff))
					this.pawn.health.RemoveHediff(hediff);
		}
	}
}
