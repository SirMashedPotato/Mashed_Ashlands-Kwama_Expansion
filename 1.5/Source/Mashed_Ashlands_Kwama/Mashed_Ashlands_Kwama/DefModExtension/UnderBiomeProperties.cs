using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class UnderBiomeProperties : DefModExtension
    {
        public bool increaseCavePlantWeight = true;
        public List<BiomePlantRecord> wildPlants;
        public List<BiomeAnimalRecord> wildAnimals;

        public static UnderBiomeProperties Get(Def def)
        {
            return def.GetModExtension<UnderBiomeProperties>();
        }
    }
}
