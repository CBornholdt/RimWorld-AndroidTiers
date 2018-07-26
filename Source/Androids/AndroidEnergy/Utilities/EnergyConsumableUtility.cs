using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace MOARANDROIDS
{
    public class EnergyConsumableUtility
    {
        static public bool IsConsumableComp(CompProperties comp) =>
            typeof(CompProperties_EnergyConsumable).IsAssignableFrom(comp.GetType());

		static public float GetConsumableEnergyFor(ThingDef def) =>
			(def.comps.FirstOrDefault(IsConsumableComp) as CompProperties_EnergyConsumable)
				?.energyAmount ?? 0;
    
        static public IEnumerable<ThingComp_EnergyConsumable> PotentialConsumableEnergySources(Pawn pawn
            , bool criticalSearch) => PotentialConsumableEnergySources(pawn, pawn, criticalSearch);

        static public IEnumerable<ThingComp_EnergyConsumable> PotentialConsumableEnergySources(Pawn getter
            , Pawn user, bool criticalSearch)
        {
            var energyNeed = user.needs.TryGetNeed<Need_Energy>();
            if(energyNeed == null)
                return Enumerable.Empty<ThingComp_EnergyConsumable>();

            Func<ThingComp_EnergyConsumable, bool> validator;
            if(!criticalSearch) {
                float energyLimit = energyNeed.MaxLevel - energyNeed.CurLevel;
                validator = eComp => eComp != null
                                            && eComp.Props.energyAmount <= energyLimit
                                            && eComp.parent.IngestibleNow
                                            && !eComp.parent.IsForbidden(getter)
                                            && getter.CanReserveAndReach(eComp.parent, PathEndMode.Touch, Danger.Deadly
                                                , maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false);
            }
            else
                validator = eComp => eComp != null && eComp.parent.IngestibleNow
                                            && !eComp.parent.IsForbidden(getter)
                                            && getter.CanReserveAndReach(eComp.parent, PathEndMode.Touch, Danger.Deadly
                                                , maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false);
            
            return getter.Map.listerThings.ThingsInGroup(ThingRequestGroup.HaulableAlways)
                         .Select(thing => thing.TryGetComp<ThingComp_EnergyConsumable>())
                         .Where(validator);                  
        }

		static public int WillIngestStackCountOf(Pawn ingester, ThingDef def)
		{
			float neededEnergy = ingester.needs.TryGetNeed<Need_Energy>()?.EnergyNeeded ?? 0;
			if(neededEnergy < GenMath.BigEpsilon)
				return 0;

			float energyPerThing = GetConsumableEnergyFor(def);
			if(energyPerThing < GenMath.BigEpsilon)
				return 0;

			return (int) Mathf.Clamp(Mathf.RoundToInt(neededEnergy / energyPerThing), 1, def.ingestible.maxNumToIngestAtOnce);
		}
    }
}
