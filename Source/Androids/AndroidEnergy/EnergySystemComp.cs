using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public abstract class EnergySystemComp : ThingComp
    {
		public EnergySystem energySystem;
    
        abstract public float AttachPriority { get; }
        
		virtual public void ApplyEffects() { }
		virtual public IEnumerable<Gizmo> GetEnergyGizmos() { yield break; }
		virtual public void Attached(EnergySystem energySystem) => this.energySystem = energySystem;
		virtual public void Removed() => this.energySystem = null;      
    }
}
