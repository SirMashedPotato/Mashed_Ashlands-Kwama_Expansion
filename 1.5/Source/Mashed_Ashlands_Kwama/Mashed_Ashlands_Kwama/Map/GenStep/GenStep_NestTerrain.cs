using Verse;
using Verse.Noise;

namespace Mashed_Ashlands_Kwama 
{
    public class GenStep_NestTerrain : GenStep
    {

        public override int SeedPart => 1921024373;

        public override void Generate(Map map, GenStepParams parms)
        {
            UnderBiomeProperties props = UnderBiomeProperties.Get(map.Biome);
            if (props == null)
            {
                Log.Error("UnderBiomeProperties in " + map.Biome + " is missing");
                return;
            }
            else
            {
                Perlin perlin = new Perlin(0.07999999821186066, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
                Perlin perlin2 = new Perlin(0.1599999964237213, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
                MapGenFloatGrid caves = MapGenerator.Caves;
                foreach (IntVec3 cell in map.AllCells)
                {
                    if (!(caves[cell] <= 0f) && !cell.GetTerrain(map).IsRiver)
                    {
                        float num = (float)perlin.GetValue(cell.x, 0.0, cell.z);
                        float num2 = (float)perlin2.GetValue(cell.x, 0.0, cell.z);
                        if (num > props.waterThreshold)
                        {
                            map.terrainGrid.SetTerrain(cell, props.waterTerrain);
                        }
                        else if (num2 > props.gravelThreshold)
                        {
                            map.terrainGrid.SetTerrain(cell, props.gravelTerrain);
                        }
                    }
                }
            }
        }
    }
}
