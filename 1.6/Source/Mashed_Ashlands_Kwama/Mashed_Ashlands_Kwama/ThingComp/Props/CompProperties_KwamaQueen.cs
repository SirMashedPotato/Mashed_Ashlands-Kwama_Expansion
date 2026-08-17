using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class CompProperties_KwamaQueen : CompProperties
    {
        public CompProperties_KwamaQueen()
        {
            compClass = typeof(Comp_KwamaQueen);
        }

        public int intervalDays = 1;
        public SoundDef sacSpawnSound;
        public PawnKindDef warriorKind;
        public PawnKindDef workerKind;
    }
}
