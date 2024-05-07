using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class GenStep_NestChambers : GenStep
    {
        public IntRange chamberSizeRange;
        public IntRange chamberCountRange;

        public override int SeedPart => 75889013;

        public override void Generate(Map map, GenStepParams parms)
        {
            int chamberCount = chamberCountRange.RandomInRange;
            for (int i = 0; i < chamberCount; i++)
            {
                int chamberSize = chamberSizeRange.RandomInRange;
                CellFinder.TryFindRandomCell(map, (IntVec3 c) => c.Standable(map) && c.DistanceToEdge(map) > 10, out IntVec3 result);
                List<IntVec3> list = GridShapeMaker.IrregularLump(result, map, chamberSize);
                foreach (IntVec3 item in list)
                {
                    foreach (Thing item2 in from t in item.GetThingList(map).ToList()
                                            where t.def.destroyable
                                            select t)
                    {
                        item2.Destroy();
                    }
                }
            }
        }
    }
}
