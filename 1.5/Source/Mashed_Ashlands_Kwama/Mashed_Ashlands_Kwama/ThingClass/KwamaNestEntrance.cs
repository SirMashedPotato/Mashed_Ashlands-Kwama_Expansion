using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Mashed_Ashlands_Kwama
{
    [StaticConstructorOnStartup]
    public class KwamaNestEntrance : MapPortal
    {
        public override IntVec3 GetDestinationLocation()
        {
            throw new NotImplementedException();
        }

        public override Map GetOtherMap()
        {
            throw new NotImplementedException();
        }
    }
}
