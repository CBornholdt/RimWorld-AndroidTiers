using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Alert_LowEnergyColonists : Alert
    {
        private IEnumerable<Pawn> LowEnergyColonists {
            get {
				return PawnsFinder.AllMaps_FreeColonistsSpawned
					.Where(pawn => (pawn.needs.TryGetNeed<Need_Energy>()?.EnergyNeed ?? EnergyNeedCategory.None)
								   >= EnergyNeedCategory.Major);
			}
        }

		public override AlertPriority Priority {
			get {
				if(LowEnergyColonists.Any(pawn => pawn.needs
						.TryGetNeed<Need_Energy>().EnergyNeed >= EnergyNeedCategory.Critical))
					return AlertPriority.Critical;
				else
					return AlertPriority.High;
			}
		}

		public Alert_LowEnergyColonists()
        {
            this.defaultLabel = "AT.Alert.LowEnergy.Label".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn current in this.LowEnergyColonists)
            {
                stringBuilder.AppendLine("    " + current.NameStringShort);
            }
            return string.Format("AT.Alert.LowEnergy.Explanation".Translate(), stringBuilder.ToString());
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritIs(this.LowEnergyColonists.FirstOrDefault<Pawn>());
        }
    }
}