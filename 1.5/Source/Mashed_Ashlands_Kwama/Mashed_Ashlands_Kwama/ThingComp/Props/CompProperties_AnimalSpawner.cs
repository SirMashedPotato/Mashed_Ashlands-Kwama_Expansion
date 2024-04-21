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
    }
}
