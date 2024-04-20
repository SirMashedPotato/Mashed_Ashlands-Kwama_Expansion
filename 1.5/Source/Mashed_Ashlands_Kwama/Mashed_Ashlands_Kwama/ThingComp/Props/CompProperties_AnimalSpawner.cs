using System.Collections.Generic;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class CompProperties_AnimalSpawner : CompProperties
    {
        public CompProperties_AnimalSpawner()
        {
            compClass = typeof(Comp_AnimalSpawner);
        }

        public int initialSpawnCount = 2;
        public int tickInterval = 10;
        public List<AnimalSpawns> spawnDefs;
    }

    public class AnimalSpawns 
    {
        public PawnKindDef kindDef;
        public float weight = 1f;
        public int maxOnMap;
    }

}
