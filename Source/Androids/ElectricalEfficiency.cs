using System.Collections.Generic;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x0200044A RID: 1098
    public class PawnCapacityWorker_ElectricalEfficiency : PawnCapacityWorker
    {
        // Token: 0x0600127F RID: 4735 RVA: 0x0008E848 File Offset: 0x0008CC48
        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            BodyDef body = diffSet.pawn.RaceProps.body;
            string tag;
            if (body.HasPartWithTag("EVKidney"))
            {
                tag = "EVKidney";
                float num = PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, float.MaxValue, impactors);
                tag = "EVLiver";
                return num * PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, float.MaxValue, impactors);
            }
            tag = "EVSource";
            return PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, float.MaxValue, impactors);
        }

        // Token: 0x06001280 RID: 4736 RVA: 0x0008E8BD File Offset: 0x0008CCBD
        public override bool CanHaveCapacity(BodyDef body)
        {
            return (body.HasPartWithTag("EVKidney") && body.HasPartWithTag("EVLiver")) || body.HasPartWithTag("EVSource");
        }
    }
}