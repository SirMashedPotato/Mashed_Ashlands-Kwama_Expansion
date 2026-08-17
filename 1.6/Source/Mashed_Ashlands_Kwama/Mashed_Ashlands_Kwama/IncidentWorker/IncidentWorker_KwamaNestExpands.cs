using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class IncidentWorker_KwamaNestExpands : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return base.CanFireNowSub(parms) && map.listerThings.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestEntrance).Count == 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!LargeBuildingCellFinder.TryFindCell(out var cell, map, NestEntranceSpawnParms.ForThing(ThingDefOf.Mashed_Ashlands_KwamaNestEntrance)))
            {
                return false;
            }

            BuildingGroundSpawner buildingGroundSpawner = (BuildingGroundSpawner)ThingMaker.MakeThing(ThingDefOf.Mashed_Ashlands_KwamaNestEntranceSpawner);
            GenSpawn.Spawn(buildingGroundSpawner, cell, map);
            SendStandardLetter(parms, buildingGroundSpawner);
            return true;
        }

        public static readonly LargeBuildingSpawnParms NestEntranceSpawnParms = new LargeBuildingSpawnParms
        {
            ignoreTerrainAffordance = true,
            allowFogged = false,
            attemptNotUnderBuildings = true
        };
    }
}
