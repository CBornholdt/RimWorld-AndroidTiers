using System;
using System.Text;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class StatWorker_ConsumableEnergyAmount : StatWorker
    {
		public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
		{
			var energyConsumable = req.Thing?.TryGetComp<ThingComp_EnergyConsumable>();
			if(energyConsumable == null) {
				Log.ErrorOnce($"Attempted to get ConsumableEnergyAmount for invalid thing {req.Thing?.ToString() ?? "NULL"}", 312);
				return 0;
			}
			return energyConsumable.Props.energyAmount;   
		}

		public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense) => "AT.Stats.ConsumableEnergyAmount.Explanation".Translate();

		public override bool ShouldShowFor(BuildableDef def)
        {
            ThingDef thingDef = def as ThingDef;
			return thingDef != null && thingDef.comps.Any(prop => prop is CompProperties_EnergyConsumable);
        }
	}
}
