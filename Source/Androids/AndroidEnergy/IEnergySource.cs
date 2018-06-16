using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
	public enum SourceStatusType : byte { NotActive, Active };

    public interface IEnergySource : ILoadReferenceable
    {
		SourceStatusType SourceStatus { get; }
        float SourcePriority { get; }
        float DesiredSourceRatePer1000Ticks { get; }
		float TrySourceEnergy(float amount);
    }
}
