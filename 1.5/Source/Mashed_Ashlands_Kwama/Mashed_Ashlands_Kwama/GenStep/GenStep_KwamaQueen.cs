using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class GenStep_KwamaQueen : GenStep
    {
        private const int chamberSize = 40;
        private const float minSpawnDistFromNestExit = 60f;
        private static readonly IntRange numEggSacsInRange = new IntRange(6, 12);

        public override int SeedPart => 23330778;

        public override void Generate(Map map, GenStepParams parms)
        {
            KwamaNestExit nestExit = (KwamaNestExit)map.listerThings.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestExit).First();
            CellFinder.TryFindRandomCell(map, (IntVec3 c) => c.Standable(map) && !c.InHorDistOf(nestExit.Position, minSpawnDistFromNestExit) && c.DistanceToEdge(map) > 10, out IntVec3 result);
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

            GenSpawn.Spawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Mashed_Ashlands_KwamaQueen)), result, map, Rot4.Random);

            int eggSacCount = numEggSacsInRange.RandomInRange;
            for (int i = 0; i < eggSacCount; i++)
            {
                if (CellFinder.TryFindRandomCellNear(result, map, 5, (IntVec3 c) => c.Standable(map) && !c.GetTerrain(map).IsWater, out IntVec3 eggSacSpawn))
                {
                    GenSpawn.Spawn(KwamaUtility.GetKwamaEggType(), eggSacSpawn, map);
                }
            }
        }
    }
}
