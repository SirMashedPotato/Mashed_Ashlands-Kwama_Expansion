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
        public Map kwamaNest;
        public int kwamaNestSize = 100;

        private PitGateExit pitGateExit;

        public void GenerateKwamaNest()
        {
            kwamaNest = PocketMapUtility.GeneratePocketMap(new IntVec3(kwamaNestSize, 1, kwamaNestSize), MapGeneratorDefOf.Mashed_Ashlands_KwamaNest, null, Map);
            pitGateExit = kwamaNest.listerThings.ThingsOfDef(ThingDefOf.PitGateExit).First() as PitGateExit;
            //pitGateExit.pitGate = this;
        }

        public override IntVec3 GetDestinationLocation()
        {
            return pitGateExit?.Position ?? IntVec3.Invalid;
        }

        public override Map GetOtherMap()
        {
            if (kwamaNest == null)
            {
                GenerateKwamaNest();
            }
            return kwamaNest;
        }
    }
}
