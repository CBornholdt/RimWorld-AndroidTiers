using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Harmony;
using UnityEngine;

namespace MOARANDROIDS
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    static public class FloatMenuMakerMap_AddHumanlikeOrders
    {
		static public void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			if(pawn.IsAndroid())
				AddAndroidOrders(clickPos, pawn, opts);
		}
    
        static public void AddAndroidOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            IntVec3 c = IntVec3.FromVector3(clickPos);

			var energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
			if(energyNeed == null)
				return;

			MenuOptionPriority menuPriorityForEnergy;
			switch(energyNeed.EnergyNeed) {
				case EnergyNeedCategory.Critical:
					menuPriorityForEnergy = MenuOptionPriority.High;
					break;
				case EnergyNeedCategory.None:
					menuPriorityForEnergy = MenuOptionPriority.VeryLow;
					break;
				case EnergyNeedCategory.Minor:
					menuPriorityForEnergy = MenuOptionPriority.Low;
					break;
				default:
					menuPriorityForEnergy = MenuOptionPriority.Default;
					break;
			}

			foreach(Thing current in c.GetThingList(pawn.Map)) {
				Thing t = current;
                
				var consumableComp = t.TryGetComp<ThingComp_EnergyConsumable>();
				if(consumableComp != null) {
                
					string text;
					if(t.def.ingestible.ingestCommandString.NullOrEmpty())
						text = "ConsumeThing".Translate(t.LabelShort);
					else
						text = string.Format(t.def.ingestible.ingestCommandString, t.LabelShort);

					FloatMenuOption consumableMenuOption;

					if(!pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
						consumableMenuOption = new FloatMenuOption(text + " (" + "NoPath".Translate() + ")", action: null);
					else 
                        consumableMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, delegate {
                            t.SetForbidden(false, true);
							Job job = EnergyUtility.CreateEnergyConsumableJob(pawn, t);
                            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        }, menuPriorityForEnergy), pawn, t, "ReservedBy");
                        
					opts.Add(consumableMenuOption);
				}
                
                var batteryComp = t.TryGetComp<EnergyAdapter_PowerBattery>();
                if(batteryComp != null) {
					string text = "AT.Job.ConnectBattery.OptionLabel".Translate(t.Label);

                    FloatMenuOption batteryMenuOption;

                    if(!pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                        batteryMenuOption = new FloatMenuOption(text + " (" + "NoPath".Translate() + ")", action: null);
                    else {                                                     
                        DisconnectWhenTag whenToDisconnect = DisconnectWhenTag.NotTouching 
                                                            | DisconnectWhenTag.EnergySystemFull
                                                            | DisconnectWhenTag.SourceSlow;
						if(!AT_Mod.settings.energySearchSettings.allowCriticalToOverrideProtection
								|| energyNeed.EnergyNeed != EnergyNeedCategory.Critical)
							whenToDisconnect = whenToDisconnect | DisconnectWhenTag.SourceLow;
                                                                                                                                                                
                        batteryMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, delegate {
                            Job job = EnergyUtility.CreateConnectSourceThenDisconnectJob(pawn, t, whenToDisconnect);
                            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        }, menuPriorityForEnergy), pawn, t, "ReservedBy");
                    }
                    opts.Add(batteryMenuOption);
                }
			}
        }
    }
}
