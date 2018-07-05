using System;
using Verse;
using RimWorld;


namespace MOARANDROIDS
{
    public class IngestionOutcomeDoer_EnergyConsumable : IngestionOutcomeDoer
    {
        public IngestionOutcomeDoer_EnergyConsumable()
        {
        }

		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			var energyConsumableComp = ingested.TryGetComp<ThingComp_EnergyConsumable>();
			if(energyConsumableComp == null) {
				Log.Message($"Attempted to ingest {ingested.Label} with {pawn.Name} for energy, but it did not have a ThingComp_EnergyConsumable");
				return;
			}
			var energySystem = pawn.TryGetComp<EnergySystem>();
			if(energySystem == null) {
				Log.Message($"Attempted to ingest {ingested.Label} with {pawn.Name} for energy but they do not have an EnergySystem");
				return;
			}

			energySystem.AddEnergyDirect(energyConsumableComp.Props.energyAmount);  
		}
	}
}
