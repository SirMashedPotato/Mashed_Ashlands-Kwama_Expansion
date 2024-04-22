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
        private static readonly CachedTexture EnterPitGateTex = new CachedTexture("UI/Commands/EnterPitGate");
        private static readonly CachedTexture ViewUndercaveTex = new CachedTexture("UI/Commands/ViewUndercave");
        public Map kwamaNest;
        public int kwamaNestSize = 100;
        private KwamaNestExit nestExit;

        protected override Texture2D EnterTex => EnterPitGateTex.Texture;

        public void GenerateKwamaNest()
        {
            RandomMapGenDef randomMapGenDef = RandomMapGenDef.Get(def);
            kwamaNest = PocketMapUtility.GeneratePocketMap(new IntVec3(kwamaNestSize, 1, kwamaNestSize), randomMapGenDef.mapGenerators.RandomElement(), null, Map);
            nestExit = kwamaNest.listerThings.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestExit).First() as KwamaNestExit;
            nestExit.nestEntrance = this;
        }

        public override IntVec3 GetDestinationLocation()
        {
            return nestExit?.Position ?? IntVec3.Invalid;
        }

        public override Map GetOtherMap()
        {
            if (kwamaNest == null)
            {
                GenerateKwamaNest();
            }
            return kwamaNest;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref kwamaNest, "kwamaNestMap");
            Scribe_References.Look(ref nestExit, "kwamaNestExit");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (kwamaNest != null)
            {
                yield return new Command_Action
                {
                    defaultLabel = "ViewUndercave".Translate(),
                    defaultDesc = "ViewUndercaveDesc".Translate(),
                    icon = ViewUndercaveTex.Texture,
                    action = delegate
                    {
                        CameraJumper.TryJumpAndSelect(nestExit);
                    }
                };
            }
            if (DebugSettings.ShowDevGizmos)
            {
                if (kwamaNest != null)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Destroy inner map",
                        action = delegate 
                        {
                            PocketMapUtility.DestroyPocketMap(kwamaNest);
                            kwamaNest = null;

                        }
                    };
                    
                }
            }
            /*
            yield return new Command_Action
            {
                defaultLabel = "DEV: Collapse Pit Gate",
                action = BeginCollapsing
            };
            */
        }
    }
}
