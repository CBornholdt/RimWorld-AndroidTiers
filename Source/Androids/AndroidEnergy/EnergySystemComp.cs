using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;

namespace MOARANDROIDS
{
    public abstract class EnergySystemComp : ThingComp
    {
		public EnergySystem energySystem;
    
        abstract public float AttachPriority { get; }

		public override void PostExposeData()
		{
            if(Scribe.mode == LoadSaveMode.LoadingVars)
                Traverse.Create(Scribe.loader.crossRefs).Field("loadedObjectDirectory")
                    .Method("RegisterLoaded", new object[1] { this }).GetValue();
        
			Scribe_References.Look<EnergySystem>(ref this.energySystem, "EnergySystem");
		}

		virtual public void ApplyEffects() { }
		virtual public IEnumerable<Gizmo> GetEnergyGizmos() { yield break; }
		virtual public void Installed(EnergySystem energySystem) => this.energySystem = energySystem;
		virtual public void Removed() => this.energySystem = null;      
    }
}
