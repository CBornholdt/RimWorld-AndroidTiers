using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    public class ThinkNode_ConditionalLowEnergy : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
			Need_Energy eNeed = pawn.needs.TryGetNeed<Need_Energy>();
			return eNeed != null && eNeed.EnergyNeed >= EnergyNeedCategory.Major;
        }
    }
}
