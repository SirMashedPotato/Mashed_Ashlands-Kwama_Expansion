﻿using RimWorld;
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
        public static ThingDef Mashed_Ashlands_KwamaNestEntrance;
        public static ThingDef Mashed_Ashlands_KwamaNestEntranceSpawner;

        [MayRequireAnomaly]
        public static ThingDef Mashed_Ashlands_KwamaNestExit;
        [MayRequireAnomaly]
        public static ThingDef Mashed_Ashlands_KwamaBurrow;

        public static ThingDef Mashed_Ashlands_KwamaEggSac;
        public static ThingDef Mashed_Ashlands_GoldKwamaEggSac;
    }
}