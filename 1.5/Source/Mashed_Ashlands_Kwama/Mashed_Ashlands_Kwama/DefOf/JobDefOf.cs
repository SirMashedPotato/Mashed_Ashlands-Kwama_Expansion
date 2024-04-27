using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    [DefOf]
    public static class JobDefOf
    {
        static JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
        }
        public static JobDef Mashed_Ashlands_MoveEggSac;
    }
}