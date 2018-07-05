using System;
using Verse;

namespace MOARANDROIDS
{
	public enum SinkStatusType : byte { Disabled = 0, Passive = 1, Active = 2 };

    public interface IEnergySink : ILoadReferenceable
    {
		SinkStatusType SinkStatus { get; }
		float SinkPriority { get; }
		float DesiredSinkRatePer1000Ticks { get; }
        float CurrentMaxSinkableEnergy { get; }
		void SinkEnergy(float amount);
		void SinkAttached(EnergySystem system);
		void SinkDetached(EnergySystem system);
    }
}
