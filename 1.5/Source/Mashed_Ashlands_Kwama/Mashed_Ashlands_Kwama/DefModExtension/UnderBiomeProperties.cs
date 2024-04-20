using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class UnderBiomeProperties : DefModExtension
    {
        public TerrainDef gravelTerrain;
        public float gravelThreshold = 0.55f;
        public TerrainDef waterTerrain;
        public float waterThreshold = 0.93f;

        public bool increaseCavePlantWeight = true;
        public List<BiomePlantRecord> wildPlants;

        public static UnderBiomeProperties Get(Def def)
        {
            return def.GetModExtension<UnderBiomeProperties>();
        }
    }
}
