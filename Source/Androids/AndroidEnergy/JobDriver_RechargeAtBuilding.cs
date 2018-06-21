using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace MOARANDROIDS
{
    public class JobDriver_RechargeAtBuilding : JobDriver
    {
		public readonly TargetIndex BatteryInd = TargetIndex.A;
		public readonly float PowerWithdrawnPerTick = 1f;
    
		override public bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.job.GetTarget(BatteryInd).Thing, this.job);
		}
    
		override protected IEnumerable<Toil> MakeNewToils()
		{
            this.FailOnDespawnedNullOrForbidden (TargetIndex.A);
            yield return Toils_Goto.GotoThing (TargetIndex.A, PathEndMode.Touch);

			CompPowerBattery batteryComp = TargetA.Thing.TryGetComp<CompPowerBattery>();
			Need_Energy energyNeed = this.pawn.needs.TryGetNeed<Need_Energy>();
			Toil rechargeToil = new Toil();
			rechargeToil.defaultCompleteMode = ToilCompleteMode.Never;
			rechargeToil.tickAction = delegate {
				float amountToDraw = Mathf.Min(batteryComp.StoredEnergy, PowerWithdrawnPerTick);
				batteryComp.DrawPower(amountToDraw);
				energyNeed.AddPower(amountToDraw);
				if(energyNeed.CurLevel == energyNeed.MaxLevel || batteryComp.StoredEnergy < PowerWithdrawnPerTick)
					this.ReadyForNextToil();
			};
			yield return rechargeToil;
		}
    }
}
