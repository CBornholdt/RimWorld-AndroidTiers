using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public abstract class HediffComp_AndroidImplant : HediffComp
    {
        abstract public void LoadSettingsFromThingComp(ThingComp_AndroidImplant implant);
    }
}
