using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MOARANDROIDS
{
    public class CapacityWorker_Conduit : PawnCapacityWorker
    {
        public override bool CanHaveCapacity(BodyDef body) =>
            body.AllParts.Any(part => part.def == AndroidParts.MConduit);

        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            return base.CalculateCapacityLevel(diffSet, impactors);
        }
    }
}
