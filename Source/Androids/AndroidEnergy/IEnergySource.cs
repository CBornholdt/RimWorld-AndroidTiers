using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
	public enum SourceStatusType : byte { Disabled = 0, Passive = 1, Active = 2 };

    public interface IEnergySource : ILoadReferenceable
    {
		SourceStatusType SourceStatus { get; }
        float SourcePriority { get; }
        float DesiredSourceRatePer1000Ticks { get; }
        float CurrentMaxSourcableEnergy { get; }
		void SourceEnergy(float amount);
    }
}
