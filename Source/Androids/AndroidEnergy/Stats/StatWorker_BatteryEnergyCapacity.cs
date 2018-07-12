using System;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class StatWorker_BatteryEnergyCapacity : StatWorker
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
			var energyStorageComp = req.Thing?.TryGetComp<EnergySystemComp_Battery>();
                
            if(energyStorageComp == null) {
                Log.ErrorOnce($"Attempted to get BatteryEnergyCapacity for invalid thing {req.Thing?.ToString() ?? "NULL"}", 312);
                return 0;
            }
            return energyStorageComp.Props.storageCapacity;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense) => "AT.Stats.BatteryEnergyCapacity.Explanation".Translate();

        public override bool ShouldShowFor(BuildableDef def)
        {
            ThingDef thingDef = def as ThingDef;
            return thingDef != null && thingDef.comps.Any(prop => prop is EnergySystemProps_Battery);
        }
    }
}
