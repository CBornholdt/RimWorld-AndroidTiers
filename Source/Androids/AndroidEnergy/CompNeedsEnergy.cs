using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class CompNeedsEnergy : ThingComp
    {
		public CompProperties_NeedsEnergy Props => (CompProperties_NeedsEnergy)this.props;
    
        public CompNeedsEnergy()
        {
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
