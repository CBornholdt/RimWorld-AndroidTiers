using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class CompNeedsEnergy : ThingComp
    {
		public CompProperties_NeedsEnergy Props => (CompProperties_NeedsEnergy)this.props;

		public Need_Energy EnergyNeed => ((Pawn)this.parent).needs.TryGetNeed<Need_Energy>();
    
        public CompNeedsEnergy()
        {
        }

		override public IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if(Prefs.DevMode && DebugSettings.godMode) {
				yield return new Command_Action() {
					defaultLabel = "DEBUG: Set Energy FULL",
					action = () => EnergyNeed.CurLevel = EnergyNeed.MaxLevel
				};
				yield return new Command_Action() {
					defaultLabel = "DEBUG: Set Energy LOW",
					action = () => EnergyNeed.CurLevel = EnergyNeed.Props.lowLevelThreshPercent
				};
				yield return new Command_Action() {
					defaultLabel = "DEBUG: Set Energy CRITICAL LOW",
					action = () => EnergyNeed.CurLevel = EnergyNeed.Props.criticallyLowLevelThreshPercent
				};
				yield return new Command_Action() {
					defaultLabel = "DEBUG: Set Energy EMPTY",
					action = () => EnergyNeed.CurLevel = EnergyNeed.Props.criticallyLowLevelThreshPercent
				};
			}
		}
        

    //Think there is a bug in GatheringsUtility.ShouldGuestKeepAttendingGathering
	/*	override public void PostSpawnSetup(bool respawningAfterLoad)
		{
			Pawn p = this.parent as Pawn;
			Log.Message(this.parent.ToString() + " foods:" + (p.needs.food == null).ToString() + " bleedRate:"
							   + p.health.hediffSet.BleedRateTotal + " rest:" + (p.needs.rest == null).ToString());
		}   */
	}
}
