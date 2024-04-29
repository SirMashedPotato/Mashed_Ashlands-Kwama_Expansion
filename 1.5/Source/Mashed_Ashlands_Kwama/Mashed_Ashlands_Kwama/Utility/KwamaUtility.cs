using Verse;

namespace Mashed_Ashlands_Kwama
{
    internal static class KwamaUtility
    {
        public static ThingDef GetKwamaEggType()
        {
            return Rand.Chance(0.95f) ? ThingDefOf.Mashed_Ashlands_KwamaEggSac : ThingDefOf.Mashed_Ashlands_GoldKwamaEggSac;
        }
    }
}
