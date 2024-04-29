using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    [DefOf]
    public static class SoundDefOf
    {
        static SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
        }
        public static SoundDef BuildingDestroyed_Stone_Big;
    }
}