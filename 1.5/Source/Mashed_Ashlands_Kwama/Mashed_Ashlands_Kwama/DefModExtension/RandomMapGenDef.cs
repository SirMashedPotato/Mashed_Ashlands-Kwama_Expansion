using System.Collections.Generic;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class RandomMapGenDef : DefModExtension
    {
        public List<MapGeneratorDef> mapGenerators;

        public static RandomMapGenDef Get(Def def)
        {
            return def.GetModExtension<RandomMapGenDef>();
        }
    }
}
