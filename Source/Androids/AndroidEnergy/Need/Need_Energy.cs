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
        private static readonly float Epsilon = 0.00001f;
    
		public Need_Energy() => this.threshPercents = new List<float>(3);

		public CompProperties_NeedsEnergy Props => this.pawn.TryGetComp<CompNeedsEnergy>()?.Props;

		public EnergySystem EnergySystem => this.pawn.TryGetComp<EnergySystem>();

		public float EnergyNeeded => this.MaxLevel - this.CurLevel;
        
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
            
            if(Props.criticallyLowLevelHediff != null) {
                if(eNeeds == EnergyNeedCategory.Critical && this.CurLevel >= Epsilon) {
                    if(!this.pawn.health.hediffSet.HasHediff(Props.criticallyLowLevelHediff))
                        this.pawn.health.AddHediff(Props.criticallyLowLevelHediff);
                }   //Check for hediff removal
                else if(this.pawn.health.hediffSet.HasHediff(Props.criticallyLowLevelHediff))
                    pawn.RemoveAllHediffsWhere(hediff => hediff.def == Props.criticallyLowLevelHediff);
            }

			if(Props.lowLevelHediff != null) {
				if(eNeeds == EnergyNeedCategory.Major) {
					if(!this.pawn.health.hediffSet.HasHediff(Props.lowLevelHediff))
						this.pawn.health.AddHediff(Props.lowLevelHediff);
				}   //Check for hediff removal
				else if(this.pawn.health.hediffSet.HasHediff(Props.lowLevelHediff))
					pawn.RemoveAllHediffsWhere(hediff => hediff.def == Props.lowLevelHediff);
			}
            
            //Should be last, if this kills the pawn other hediffs might not be removed
            if(Props.emptyLevelHediff != null) {    
                if(this.CurLevel < Epsilon) {
                    if(!this.pawn.health.hediffSet.HasHediff(Props.emptyLevelHediff))
                        this.pawn.health.AddHediff(Props.emptyLevelHediff);
                }   //Check for hediff removal
                else if(this.pawn.health.hediffSet.HasHediff(Props.emptyLevelHediff))
                    pawn.RemoveAllHediffsWhere(hediff => hediff.def == Props.emptyLevelHediff);
            }
		}
	}
}
