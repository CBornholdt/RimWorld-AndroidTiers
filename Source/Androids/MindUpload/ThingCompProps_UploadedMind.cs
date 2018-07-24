using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class ThingCompProps_UploadedMind : CompProperties_AndroidImplant
    {
        public ThingCompProps_UploadedMind()
        {
			this.compClass = typeof(ThingComp_UploadedMind);
        }
    }
}
