using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class ThingComp_UploadedMind : ThingComp_AndroidImplant
    {
        public CopyablePawn mindSource;

        public ThingComp_UploadedMind() { }

		public override void LoadStateFromHediffComp(HediffComp_AndroidImplant implant)
		{
			base.LoadStateFromHediffComp(implant);
		}
	}
}