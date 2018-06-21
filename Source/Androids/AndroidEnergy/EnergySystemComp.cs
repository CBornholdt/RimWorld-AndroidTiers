using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;

namespace MOARANDROIDS
{
    public abstract class EnergySystemComp : ThingComp, ILoadReferenceable
    {
		public EnergySystem energySystem;
    
        abstract public float AttachPriority { get; }

		abstract public string GetUniqueLoadID();

		public override void PostExposeData()
		{
			this.ForceRegisterReferenceable();
			Scribe_References.Look<EnergySystem>(ref this.energySystem, "EnergySystem");
		}

		virtual public void ApplyEffects() { }
		virtual public IEnumerable<Gizmo> GetEnergyGizmos() { yield break; }
		virtual public void Installed(EnergySystem energySystem) => this.energySystem = energySystem;
		virtual public void Removed() => this.energySystem = null;      
    }
}
