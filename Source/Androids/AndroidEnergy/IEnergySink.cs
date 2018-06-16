using System;
using Verse;

namespace MOARANDROIDS
{
	public enum SinkStatusType : byte { NotActive, Active };

    public interface IEnergySink : ILoadReferenceable
    {
		SinkStatusType SinkStatus { get; }
		float SinkPriority { get; }
		float DesiredSinkRatePer1000Ticks { get; }
		float TrySinkEnergy(float amount);
    }
}
