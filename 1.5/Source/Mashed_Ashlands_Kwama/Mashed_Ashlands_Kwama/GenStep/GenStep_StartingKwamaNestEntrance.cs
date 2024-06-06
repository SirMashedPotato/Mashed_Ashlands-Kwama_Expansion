using Mashed_Ashlands;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    internal class GenStep_StartingKwamaNestEntrance : GenStep_ScatterAshlandsSpecific
    {
        public override void Generate(Map map, GenStepParams parms)
        {
            if (Mashed_Ashlands_Kwama_ModSettings.EnableStartingKwamaNestEntrance)
            {
                base.Generate(map, parms);
            }
        }
    }
}
