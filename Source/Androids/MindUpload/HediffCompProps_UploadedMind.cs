using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class HediffCompProps_UploadedMind : HediffProperties_AndroidImplant
    {
        public HediffCompProps_UploadedMind()
        {
            this.compClass = typeof(ThingComp_UploadedMind);
        }
    }
}
