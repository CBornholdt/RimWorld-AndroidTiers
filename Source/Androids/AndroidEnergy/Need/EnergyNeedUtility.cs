using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    static public class EnergyNeedUtility
    {
		static public EnergyNeedCategory CurrentEnergyNeed(this Pawn pawn) =>
			pawn.needs.TryGetNeed<Need_Energy>()?.EnergyNeed ?? EnergyNeedCategory.None;

		static public bool EnergyNeedAtLeast(this Pawn pawn, EnergyNeedCategory cat) =>
			pawn.CurrentEnergyNeed() >= cat;

		static public float TotalEnergyNeeded(this Pawn pawn) =>
			pawn.needs.TryGetNeed<Need_Energy>()?.MaxLevel ?? 0f
				- pawn.needs.TryGetNeed<Need_Energy>()?.CurLevel ?? 0f;
    }
}
