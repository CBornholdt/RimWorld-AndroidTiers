using System;
using Verse;
namespace MOARANDROIDS
{
    //These are all Things
    public interface IEnergySystemConnectable
    {
		ConnectWhen WhenToConnect();
		DisconnectWhen WhenToDisconnect();

		ThingWithComps Parent { get; }

		bool IsAvailableFor(EnergySystem system, bool isCritical);
		bool IsAvailableFor(EnergySystem system, bool isCritical, out string unavailableReason);
        
        int SimultaneousConnections { get; }

		string ForcedWorkFloatMenuOptionText { get; }
    }
}
