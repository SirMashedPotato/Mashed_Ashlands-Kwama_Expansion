using Verse;

namespace Mashed_Ashlands_Kwama
{
    internal struct QueuedCellEffecter
    {
        public EffecterDef effecterDef;

        public int tick;

        public IntVec3 cell;

        public QueuedCellEffecter(EffecterDef edef, IntVec3 cell, int tick)
        {
            effecterDef = edef;
            this.tick = tick;
            this.cell = cell;
        }
    }
}
