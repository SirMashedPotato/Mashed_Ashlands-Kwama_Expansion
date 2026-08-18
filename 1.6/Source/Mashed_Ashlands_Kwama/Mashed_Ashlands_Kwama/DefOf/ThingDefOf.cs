using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    [DefOf]
    public static class ThingDefOf
    {
        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }

        public static ThingDef Mashed_Ashlands_KwamaEggSac;
        public static ThingDef Mashed_Ashlands_GoldKwamaEggSac;
        public static ThingDef Mashed_Ashlands_KwamaBurrow;
    }
}