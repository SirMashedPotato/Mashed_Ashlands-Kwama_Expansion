using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    /// <summary>
    /// Allows for custom cave plants for specific biomes.
    /// </summary>
    [HarmonyPatch(typeof(WildPlantSpawner))]
    [HarmonyPatch("CalculatePlantsWhichCanGrowAt")]
    public static class WildPlantSpawner_CalculatePlantsWhichCanGrowAt_Patch
    {

        [HarmonyPrefix]
        public static bool Mashed_AshlandsKwama_CustomCavePlants_Patch(IntVec3 c, bool cavePlants, List<ThingDef> outPlants, Map ___map)
        {
            UnderBiomeProperties props = UnderBiomeProperties.Get(___map.Biome);
            if (props != null && !props.wildPlants.NullOrEmpty())
            {
                outPlants.Clear();
                foreach(BiomePlantRecord record in props.wildPlants)
                {
                    if (record.plant.CanEverPlantAt(c, ___map))
                    {
                        outPlants.Add(record.plant);
                    }
                }
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Increases the weight of ashland cave plants in ashland biomes, specifically for cave tiles
    /// </summary>
    [HarmonyPatch(typeof(WildPlantSpawner))]
    [HarmonyPatch("PlantChoiceWeight")]
    public static class WildPlantSpawner_PlantChoiceWeight_Patch
    {
        [HarmonyPostfix]
        public static void Mashed_AshlandsKwama_PlantChoiceWeight_Patch(ThingDef plantDef, IntVec3 c, ref float __result, Map ___map)
        {
            UnderBiomeProperties props = UnderBiomeProperties.Get(___map.Biome);
            if (props != null && props.increaseCavePlantWeight)
            {
                RoofDef roof = c.GetRoof(___map);
                if (roof != null && roof.isNatural)
                {
                    __result = props.wildPlants.Find(x => x.plant == plantDef).commonality;
                }
            }
        }
    }
}
