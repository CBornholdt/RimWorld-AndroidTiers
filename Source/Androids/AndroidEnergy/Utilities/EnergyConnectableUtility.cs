using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    static public class EnergyConnectableUtility
    {
        static public IEnumerable<IEnergySystemConnectable> PotentialConnectableEnergySources(Pawn pawn) =>
            PotentialConnectableEnergySources(pawn, false);

        static public IEnumerable<IEnergySystemConnectable> PotentialConnectableEnergySources(Pawn pawn
                                                                            , bool criticalSearch)
        {
            var energySystem = pawn.TryGetComp<EnergySystem>();
            if(energySystem == null)
                return Enumerable.Empty<IEnergySystemConnectable>();

			Func<IEnergySystemConnectable, bool> validator = connectable =>
									connectable.IsAvailableFor(energySystem, criticalSearch)
										&& connectable is IEnergySource
										&& pawn.CanReserveAndReach(connectable.Parent, PathEndMode.Touch, Danger.Deadly
											 , maxPawns: connectable.SimultaneousConnections)
                                        && !energySystem.AttachedSources.Contains(connectable as IEnergySource);

            //TODO adjust this for non-colonist pawns if need be
            return pawn.Map.AllBuildingsWithComp<IEnergySystemConnectable>().Where(validator);                     
        } 
        
        static public bool ShouldWaitForDisconnection(Pawn pawn, IEnergySystemConnectable connectable)
        {
			return true;
        }
    }
}
