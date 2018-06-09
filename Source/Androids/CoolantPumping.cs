using System;
using System.Collections.Generic;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x0200044B RID: 1099
    public class PawnCapacityWorker_CoolantPumping : PawnCapacityWorker
    {
        // Token: 0x06001282 RID: 4738 RVA: 0x0008E8F8 File Offset: 0x0008CCF8
        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            string tag = "CPSource";
            return PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, float.MaxValue, impactors);
        }

        // Token: 0x06001283 RID: 4739 RVA: 0x0008E91C File Offset: 0x0008CD1C
        public override bool CanHaveCapacity(BodyDef body)
        {
            return body.HasPartWithTag("CPSource");
        }
    }
}