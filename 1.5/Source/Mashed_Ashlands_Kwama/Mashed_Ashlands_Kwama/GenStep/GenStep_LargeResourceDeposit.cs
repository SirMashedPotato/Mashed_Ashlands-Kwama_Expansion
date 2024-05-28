using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class GenStep_LargeResourceDeposit : GenStep
    {
        public List<ThingDef> resourceDeposits;

        private const int chamberSize = 90;
        private const int validationRadius = 11;

        private const int chamberSizeBackup = 60;
        private const int validationRadiusBackup = 9;

        private const int chamberSizeBackupFinal = 30;
        private const int validationRadiusBackupFinal = 6;

        public override int SeedPart => 23330778;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (resourceDeposits.NullOrEmpty())
            {
                return;
            }
            CellFinder.TryFindRandomCell(map, (IntVec3 c) => ValidLocation(c, map, validationRadius), out IntVec3 result);
            if (ValidLocation(result, map, validationRadius))
            {
                GenerateChamber(result, map, chamberSize);
                return;
            }
            ///try again in case we can still manage a hidden chamber
            Log.Message("Failed to find valid location for GenStep_LargeResourceDeposit, attempting backup plan.");
            CellFinder.TryFindRandomCell(map, (IntVec3 c) => ValidLocation(c, map, validationRadiusBackup), out result);
            if (ValidLocation(result, map, validationRadiusBackup))
            {
                GenerateChamber(result, map, chamberSizeBackup);
                return;
            }
            ///final go
            Log.Warning("Failed to find valid location for GenStep_LargeResourceDeposit. If this is still generated unfogged, please report to Mashed with a screenshot of the entire map unfogged.");
            CellFinder.TryFindRandomCell(map, (IntVec3 c) => ValidLocation(c, map, validationRadiusBackupFinal), out result);
            GenerateChamber(result, map, chamberSizeBackupFinal);
        }

        private bool ValidLocation(IntVec3 c, Map map, int validationRadius)
        {
            if (c.Standable(map))
            {
                return false;
            }
            if (c.DistanceToEdge(map) <= validationRadius)
            {
                return false;
            }
            IEnumerable<IntVec3> cells = GenRadial.RadialCellsAround(c, validationRadius, true);
            foreach (IntVec3 cell in cells)
            {
                if (cell.InBounds(map) && cell.GetEdifice(map) == null)
                {
                    return false;
                }
            }
            return true;
        }

        private void GenerateChamber(IntVec3 c, Map map, int chamberSize)
        {
            List<IntVec3> list = GridShapeMaker.IrregularLump(c, map, chamberSize);
            MapGenFloatGrid caves = MapGenerator.Caves;
            foreach (IntVec3 item in list)
            {
                foreach (Thing item2 in from t in item.GetThingList(map).ToList()
                                        where t.def.destroyable
                                        select t)
                {
                    caves[item] = 1;
                    item2.Destroy();
                }
            }
            GenSpawn.Spawn(resourceDeposits.RandomElement(), c, map);
        }
    }
}
