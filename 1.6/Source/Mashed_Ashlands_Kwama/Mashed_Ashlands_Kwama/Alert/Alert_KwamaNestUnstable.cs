using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class Alert_KwamaNestUnstable : Alert_Critical
    {
        public Alert_KwamaNestUnstable()
        {
            defaultLabel = "Mashed_Ashlands_Kwama_AlertNestCollapse_Label".Translate();
            defaultExplanation = "Mashed_Ashlands_Kwama_AlertNestCollapse_Description".Translate();
            requireAnomaly = true;
        }

        private List<GlobalTargetInfo> targets = new List<GlobalTargetInfo>();
        private KwamaNestEntrance KwamaNestEntrance => targets[0].Thing as KwamaNestEntrance;

        public override string GetLabel()
        {
            return defaultLabel + ": " + KwamaNestEntrance.TicksUntilCollapse.ToStringTicksToPeriodVerbose();
        }

        protected override Color BGColor
        {
            get
            {
                KwamaNestEntrance nestEntrance = KwamaNestEntrance;
                if (nestEntrance != null && nestEntrance.CollapseStage == 1)
                {
                    return Color.clear;
                }
                return base.BGColor;
            }
        }

        private void CalculateTargets()
        {
            targets.Clear();
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                foreach (KwamaNestEntrance item in maps[i].listerThings.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestEntrance))
                {
                    if (item.IsCollapsing)
                    {
                        targets.Add(item);
                    }
                }
            }
        }

        public override AlertReport GetReport()
        {
            CalculateTargets();
            return AlertReport.CulpritsAre(targets);
        }
    }
}
