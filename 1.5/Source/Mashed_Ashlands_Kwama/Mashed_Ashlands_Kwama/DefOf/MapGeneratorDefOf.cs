using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    [DefOf]
    public static class MapGeneratorDefOf
    {
        static MapGeneratorDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MapGeneratorDefOf));
        }
        public static MapGeneratorDef Mashed_Ashlands_KwamaNest;
    }
}