using System.Linq;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class GenStep_FindKwamaNestExit : GenStep
    {
        public const float ClearRadius = 4.5f;
        public override int SeedPart => 99017287;

        public override void Generate(Map map, GenStepParams parms)
        {
            CellFinder.TryFindRandomCell(map, (IntVec3 cell) => cell.Standable(map) && cell.DistanceToEdge(map) > 5.5f, out IntVec3 result);
            foreach (IntVec3 item in GenRadial.RadialCellsAround(result, 4.5f, useCenter: true))
            {
                foreach (Thing item2 in from t in item.GetThingList(map).ToList()
                                        where t.def.destroyable
                                        select t)
                {
                    item2.Destroy();
                }
            }
            GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Mashed_Ashlands_KwamaNestExit), result, map);
            MapGenerator.PlayerStartSpot = result;
        }
    }
}
