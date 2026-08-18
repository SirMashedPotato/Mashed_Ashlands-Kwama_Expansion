using Mashed_Ashlands;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    [StaticConstructorOnStartup]
    public class KwamaNestEntrance : Building_UndercaveEntrance
    {
        public override UndercaveTypeDef UndercaveTypeDef
        {
            get
            {
                if (base.UndercaveTypeDef == null)
                {
                    base.UndercaveTypeDef = TileMutatorWorker_UndercaveEntrance.GetMineType(Map.Tile); //TODO
                }

                return base.UndercaveTypeDef;
            }
            set
            {
                base.UndercaveTypeDef = value;
            }
        }
    }
}
