using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class GenStep_HiddenResourceDeposit : GenStep
    {
        public List<ThingDef> resourceDeposits;
        public int chamberCount = 1;

        public int chamberSize = 90;
        public int validationRadius = 11;

        public int chamberSizeBackup = 60;
        public int validationRadiusBackup = 9;

        public int chamberSizeBackupFinal = 30;
        public int validationRadiusBackupFinal = 6;

        public int chamberSizeBackupFinalReally = 9;
        public int validationRadiusBackupFinalReally = 3;

        public override int SeedPart => 23330778;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (resourceDeposits.NullOrEmpty())
            {
                return;
            }
            for (int i = 0; i < chamberCount; i++)
            {
                ThingDef resourceDeposit = resourceDeposits.RandomElement();
                if (AttemptGeneration(map, validationRadius, chamberSize, resourceDeposit))
                {
                    continue;
                }
                ///try again in case we can still manage a hidden chamber
                if (AttemptGeneration(map, validationRadiusBackup, chamberSizeBackup, resourceDeposit))
                {
                    continue;
                }
                ///and again
                if (AttemptGeneration(map, validationRadiusBackupFinal, chamberSizeBackupFinal, resourceDeposit))
                {
                    continue;
                }
                ///final resort
                AttemptGeneration(map, validationRadiusBackupFinalReally, chamberSizeBackupFinalReally, resourceDeposit, true);
            }
        }

        public bool AttemptGeneration(Map map, int validationRadius, int chamberSize, ThingDef resourceDeposit, bool forced = false)
        {
            CellFinder.TryFindRandomCell(map, (IntVec3 c) => ValidLocation(c, map, validationRadius), out IntVec3 result);
            if (forced || ValidLocation(result, map, validationRadius))
            {
                GenerateChamber(result, map, chamberSize, resourceDeposit);
                return true;
            }
            return false;
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

        private void GenerateChamber(IntVec3 c, Map map, int chamberSize, ThingDef resourceDeposit)
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
            GenSpawn.Spawn(resourceDeposit, c, map);
        }
    }
}
