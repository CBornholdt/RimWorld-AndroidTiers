using System;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class StatWorker_ReactorEnergyAmount : StatWorker
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
			var energySourceProps = (HediffProperties_EnergySource) req.Thing?.def.GetImplantedHediffDef()?.comps
					.First(prop => prop is HediffProperties_EnergySource);
				
            if(energySourceProps == null) {
                Log.ErrorOnce($"Attempted to get ReactorEnergyAmount for invalid thing {req.Thing?.ToString() ?? "NULL"}", 312);
                return 0;
            }
            return energySourceProps.desiredSourceRatePer1000Ticks * 2.5f;  //2500 ticks per hour   
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense) => "AT.Stats.ReactorEnergyAmount.Explanation".Translate();

        public override bool ShouldShowFor(BuildableDef def)
        {
			ThingDef thingDef = def as ThingDef;
			HediffDef implantedDef = thingDef?.GetImplantedHediffDef();
			return thingDef != null
				&& thingDef.comps.Any(prop => typeof(CompProperties_AndroidImplant).IsAssignableFrom(prop.GetType()))
                && implantedDef != null 
                && implantedDef.comps.Any(prop => prop is HediffProperties_EnergySource);
        }
    }
}
