using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    internal static class KwamaUtility
    {
        public static ThingDef GetKwamaEggType()
        {
            return Rand.Chance(0.95f) ? ThingDefOf.Mashed_Ashlands_KwamaEggSac : ThingDefOf.Mashed_Ashlands_GoldKwamaEggSac;
        }

        public static List<Pawn> AllOfKind(Thing queen, PawnKindDef kind)
        {
            return AllOfKind(queen.Map, kind);
        }

        public static List<Pawn> AllOfKind(Map map, PawnKindDef kind)
        {
            return map.mapPawns.AllPawnsSpawned.Where(x => x.kindDef == kind && x.Faction == null).ToList();
        }
    }
}
