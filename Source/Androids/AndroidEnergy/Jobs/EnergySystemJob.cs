using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{                                        
    public class EnergySystemJob : Job
    {
		public DisconnectWhen disconnectWhen;

		public new void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<DisconnectWhen>(ref this.disconnectWhen, "DisconnectWhen");
		}

		public EnergySystemJob(JobDef def, LocalTargetInfo targetA, LocalTargetInfo targetB = default(LocalTargetInfo)
			, LocalTargetInfo targetC = default(LocalTargetInfo)) : base(def, targetA, targetB, targetC) { }
    }
}
