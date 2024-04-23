using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class UnderBiomeProperties : DefModExtension
    {
        public List<AnimalSpawns> wildAnimals;

        public static UnderBiomeProperties Get(Def def)
        {
            return def.GetModExtension<UnderBiomeProperties>();
        }
    }

    public class AnimalSpawns 
    {
        public PawnKindDef kindDef;
        public float weight = 1f;
        public int maxOnMap;
    }
}
