﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    static public class EnergyUtility
    {
        static public Hediff MakeGainEnergyHediff(Pawn pawn, float totalEnergy, int overTicks)
        {
            Hediff_GainEnergy gainEnergy = (Hediff_GainEnergy) HediffMaker.MakeHediff(EnergyHediffs.AT_GainEnergy, pawn);
            gainEnergy.totalEnergyGain = totalEnergy;
            gainEnergy.totalTickLength = Math.Max(overTicks, 1);
            gainEnergy.ticksPassed = 0;
            return gainEnergy;
        }
        
        static public Job CreateEnergyConsumableJob(Pawn pawn, Thing consumable) =>
            new Job(JobDefOf.Ingest, consumable) {
                    count = Math.Min(consumable.def.stackLimit
                                , consumable.def.ingestible.maxNumToIngestAtOnce)
                    , takeExtraIngestibles = AT_Mod.settings.energySearchSettings.maxConsumablesToCarry            
            };
            
        static public Job CreateConnectSourceThenDisconnectJob(Pawn pawn, Thing source
            , DisconnectWhenTag whenToDisconnect = DisconnectWhenTag.Never, int atTick = int.MaxValue) =>
            new EnergySystemJob() {
                def = EnergyJobs.AT_ConnectSourceThenDisconnect,
                disconnectWhen = new DisconnectWhen(whenToDisconnect, atTick),
                targetA = source
            };              
            

        static public bool TryFindBestEnergyRechargeSource(Pawn getter, Pawn user, bool criticalSearch
                                 , out Thing rechargeSource)
        {
            rechargeSource = null;

            //Will store Target, Priority
            IEnumerable<Tuple<Thing, float>> allPotentialTargets = Enumerable.Empty<Tuple<Thing, float>>();
            
            float GetConsumablePriority(Thing cons, float energy) {
                var consumableBias = AT_Mod.settings.energySearchSettings.consumableDistanceBias;
                return cons.PositionHeld.DistanceToSquared(getter.Position)
                    / (energy * energy * consumableBias * consumableBias);
            }
            float GetPersistentPriority(Thing persistent, float availEnergy) =>
                persistent.PositionHeld.DistanceToSquared(getter.Position)
                    / (availEnergy * availEnergy);

            if(AT_Mod.settings.energySearchSettings.allowConsumables)
                allPotentialTargets = PotentialConsumableEnergySources(getter, user, criticalSearch)
                                            .Select(eComp => Tuple.Create(eComp.parent as Thing
                                                        , GetConsumablePriority(eComp.parent, eComp.Props.energyAmount)));

            if(getter == user && AT_Mod.settings.energySearchSettings.allowPowerBatteries)
                allPotentialTargets = allPotentialTargets.Concat(
                        PotentialBuildingEnergySources(user, criticalSearch)
                            .Select(bComp => Tuple.Create(bComp.parent as Thing
                                                        , GetPersistentPriority(bComp.parent
                                                                , bComp.CurrentMaxSourcableEnergy))));

            rechargeSource = allPotentialTargets.MinBy(target_priority => target_priority.Item2)?.Item1;
            if(rechargeSource == null)
                return false;

            return true;
        }

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
                float energyLimit = energyNeed.MaxLevel * (1f - energyNeed.Props.criticallyLowLevelThreshPercent);
                validator = eComp => eComp != null
                                            && eComp.Props.energyAmount <= energyLimit
                                            && eComp.parent.IngestibleNow;
            }
            else
                validator = eComp => eComp != null && eComp.parent.IngestibleNow;
            
            return getter.Map.listerThings.ThingsInGroup(ThingRequestGroup.HaulableAlways)
                         .Select(thing => thing.TryGetComp<ThingComp_EnergyConsumable>())
                         .Where(validator);                  
        }     

        static public IEnumerable<EnergyAdapter_PowerBattery> PotentialBuildingEnergySources(Pawn pawn) =>
            PotentialBuildingEnergySources(pawn, false);

        static public IEnumerable<EnergyAdapter_PowerBattery> PotentialBuildingEnergySources(Pawn pawn
                                                                            , bool criticalSearch)
        {
            var energySystem = pawn.TryGetComp<EnergySystem>();
            if(energySystem == null)
                return Enumerable.Empty<EnergyAdapter_PowerBattery>();

            Func<EnergyAdapter_PowerBattery, bool> validator;
            if(criticalSearch && AT_Mod.settings.energySearchSettings.allowCriticalToOverrideProtection) 
                validator = bComp => !bComp.WasRecentlyConnected(pawn)
                                        && pawn.CanReserveAndReach(bComp.parent, PathEndMode.Touch, Danger.Deadly
                                                , maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false) 
                                        && !energySystem.AttachedSources.Contains(bComp);
            else {
                float threshPercent = AT_Mod.settings.energySearchSettings.batteryProtectionLevel;
                validator = bComp => !bComp.WasRecentlyConnected(pawn)
                                        && pawn.CanReserveAndReach(bComp.parent, PathEndMode.Touch, Danger.Deadly
                                                , maxPawns: 1, stackCount: -1, layer: null, ignoreOtherReservations: false)
                                            && !energySystem.AttachedSources.Contains(bComp)
                                            && bComp.StoredEnergyPct > threshPercent;
            }

            return pawn.Map.AllBuildingsWithComp<EnergyAdapter_PowerBattery>().Where(validator);                     
        }                                                                            
        
        static public bool IsEnergyConsumable(this Thing thing) =>
            thing.TryGetComp<ThingComp_EnergyConsumable>() != null;

        static public bool IsBuildingEnergySource(this Building building) =>
            building.AllComps.Any(comp => comp is IEnergySource);
    }
}
