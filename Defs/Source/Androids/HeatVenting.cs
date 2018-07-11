using System.Collections.Generic;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x0200044C RID: 1100
    public class PawnCapacityWorker_HeatVenting : PawnCapacityWorker
    {
        // Token: 0x06001285 RID: 4741 RVA: 0x0008E934 File Offset: 0x0008CD34
        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            string tag = "HVSource";
            return PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, float.MaxValue, impactors) * PawnCapacityUtility.CalculateTagEfficiency(diffSet, "HVPathway", 1f, impactors);
        }

        // Token: 0x06001286 RID: 4742 RVA: 0x0008E96A File Offset: 0x0008CD6A
        public override bool CanHaveCapacity(BodyDef body)
        {
            return body.HasPartWithTag("HVSource");
        }
    }
}