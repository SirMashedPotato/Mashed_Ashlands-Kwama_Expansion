using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    [DefOf]
    public static class EffecterDefOf
    {
        static EffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
        }

        [MayRequireAnomaly]
        public static EffecterDef Mashed_Ashlands_KwamaNestAboveGroundCollapseStage1;

        [MayRequireAnomaly]
        public static EffecterDef Mashed_Ashlands_KwamaNestAboveGroundCollapseStage2;

        [MayRequireAnomaly]
        public static EffecterDef Mashed_Ashlands_KwamaNestCollapseDustCloud;
    }
}