using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public static class RechargePatientUtility
    {
        //Does not check for an energy need, presumes it already exists
        public static bool ShouldBeManuallyRecharged(Pawn p)
        {
            if (p.GetPosture() == PawnPosture.Standing)
            {
                return false;
            }
            if (p.NonHumanlikeOrWildMan())
            {
                Building_Bed building_Bed = p.CurrentBed();
                if (building_Bed == null || building_Bed.Faction != Faction.OfPlayer)
                {
                    return false;
                }
            }
            else
            {
                if (p.Faction != Faction.OfPlayer && p.HostFaction != Faction.OfPlayer)
                {
                    return false;
                }
                if (!p.InBed())
                {
                    return false;
                }
            }
            if (!HealthAIUtility.ShouldSeekMedicalRest(p))
            {
                return false;
            }
            if (p.HostFaction != null)
            {
                if (p.HostFaction != Faction.OfPlayer)
                {
                    return false;
                }
                if (p.guest != null && !p.guest.CanBeBroughtFood)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
