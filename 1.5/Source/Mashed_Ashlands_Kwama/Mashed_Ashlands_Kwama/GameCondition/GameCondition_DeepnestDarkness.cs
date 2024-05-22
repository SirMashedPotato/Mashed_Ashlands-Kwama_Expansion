using RimWorld;
using UnityEngine;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class GameCondition_DeepnestDarkness : GameCondition
    {
        public static readonly SkyColorSet DeepnestSkyColors = new SkyColorSet(new Color(0.06f, 0.06f, 0.06f), Color.white, new Color(0.6f, 0.6f, 0.6f), 1f);

        public override int TransitionTicks => 0;

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(0f, DeepnestSkyColors, 0.3f, 0f);
        }
    }
}
