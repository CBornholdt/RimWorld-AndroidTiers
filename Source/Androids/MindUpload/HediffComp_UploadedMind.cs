using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffComp_UploadedMind : HediffComp_AndroidImplant
    {
		public CopyablePawn mindSource;

		public HediffComp_UploadedMind() { }

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Deep.Look<CopyablePawn>(ref this.mindSource, "MindSource");
		}

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);
			mindSource?.CopyTo(this.Pawn);
		}

		public override void LoadStateFromThingComp(ThingComp_AndroidImplant implant)
		{
			mindSource = (implant as ThingComp_UploadedMind).mindSource;
		}   
	}
}
