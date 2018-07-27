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

			Thing rechargeSource = pawn.inventory.innerContainer.FirstOrDefault(EnergyThingExt.IsEnergyConsumable);

			if(rechargeSource == null && !EnergyUtility.TryFindBestEnergyRechargeSource(pawn, pawn, criticalSearch
								, out rechargeSource))
				return null;

			if(rechargeSource.IngestibleNow)
				return EnergyUtility.CreateEnergyConsumableJob(pawn, rechargeSource);

			if(!rechargeSource.IsEnergySource()) {
				Log.Warning("JobGiver_RechargeEnergy.TryGiveJob ... should not have reached here, TryFindBestEnergyRechargeSource should have returned false");
				return null;
			}

			var whenToDisconnect = rechargeSource.TryGetCompInterface<IEnergySystemConnectable>()
												?.WhenToDisconnect() ?? new DisconnectWhen(DisconnectWhenTag.Never);

			return EnergyUtility.CreateConnectSourceThenDisconnectJob(pawn, rechargeSource, whenToDisconnect);  
        }
    }
}
