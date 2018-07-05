using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;
using UnityEngine;

namespace MOARANDROIDS
{
    public class JobGiver_RechargeEnergy : ThinkNode_JobGiver
    {
        override public float GetPriority(Pawn pawn)
        {
            var energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
            if(energyNeed == null)
                return 0;

            //Log.Message("here for " + pawn + " " + energyNeed.CurLevel + " " + energyNeed.LowEnergyNeed);

            switch(energyNeed.EnergyNeed) {
                case EnergyNeedCategory.Minor:
                    return 1;
                case EnergyNeedCategory.Moderate:
                    return 5f;
                case EnergyNeedCategory.Major:
                    return 8f;
                case EnergyNeedCategory.Critical:
                    return 10;
                default:
                    return 0f;
            }        
        }

        override protected Job TryGiveJob(Pawn pawn)
        {
            var energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
            if((energyNeed?.EnergyNeed ?? EnergyNeedCategory.None) == EnergyNeedCategory.None)
                return null;

            var energySystem = pawn.TryGetComp<EnergySystem>();
            if(energySystem == null)
                return null;

			bool criticalSearch = energyNeed.EnergyNeed >= EnergyNeedCategory.Critical;

			if(!EnergyUtility.TryFindBestEnergyRechargeSource(pawn, pawn, criticalSearch
								, out Thing rechargeSource))
				return null;

			if(rechargeSource.IngestibleNow)
				return new Job(JobDefOf.Ingest, rechargeSource) {
					count = Math.Min(rechargeSource.def.stackLimit
								, rechargeSource.def.ingestible.maxNumToIngestAtOnce)
                    , takeExtraIngestibles = AT_Mod.settings.energySearchSettings.maxConsumablesToCarry            
				};

			if(!(rechargeSource as ThingWithComps)?.AllComps.Any(comp => comp is IEnergySource) ?? false) {
				Log.Warning($"JobGiver_RechargeEnergy.TryGiveJob ... should not have reached here, TryFindBestEnergyRechargeSource should have returned false");
				return null;
			}

            var whenToDisconnect = DisconnectWhenTag.NotTouching | DisconnectWhenTag.EnergySystemFull
                            | DisconnectWhenTag.SourceSlow | DisconnectWhenTag.SourceLow;

            return new EnergySystemJob() {
                def = EnergyJobs.AT_ConnectSourceThenDisconnect,
                disconnectWhen = new DisconnectWhen(whenToDisconnect),
                targetA = rechargeSource
            };                 
        }
    }
}
