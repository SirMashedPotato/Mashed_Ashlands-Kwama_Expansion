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

        //collapsing
        private static readonly IntRange CollapseDurationTicks = new IntRange(25000, 27500);
        private int collapseTick = -999999;
        private bool isCollapsing;

        private Sustainer collapseSustainer;
        private Effecter collapseEffecter1;
        private Effecter collapseEffecter2;

        public bool IsCollapsing => isCollapsing;

        public int CollapseStage
        {
            get
            {
                if (collapseTick - Find.TickManager.TicksGame >= 3600)
                {
                    return 1;
                }
                return 2;
            }
        }

        public int TicksUntilCollapse => collapseTick - Find.TickManager.TicksGame;

        protected override Texture2D EnterTex => EnterPitGateTex.Texture;

        public void GenerateKwamaNest()
        {
            RandomMapGenDef randomMapGenDef = RandomMapGenDef.Get(def);
            kwamaNest = PocketMapUtility.GeneratePocketMap(new IntVec3(kwamaNestSize, 1, kwamaNestSize), randomMapGenDef.mapGenerators.RandomElement(), null, Map);
            nestExit = kwamaNest.listerThings.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestExit).First() as KwamaNestExit;
            nestExit.nestEntrance = this;
            Find.LetterStack.ReceiveLetter(kwamaNest.Biome.LabelCap, kwamaNest.Biome.description, LetterDefOf.NeutralEvent, nestExit);
        }

        public void BeginCollapsing()
        {
            int randomInRange = CollapseDurationTicks.RandomInRange;
            collapseTick = Find.TickManager.TicksGame + randomInRange;
            kwamaNest?.GetComponent<KwamaNestMapComponent>().Notify_BeginCollapsing();
            isCollapsing = true;
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

        public override void Tick()
        {
            base.Tick();
            if (isCollapsing)
            {
                if (CollapseStage == 1)
                {
                    if (collapseEffecter1 == null)
                    {
                        collapseEffecter1 = EffecterDefOf.PitGateAboveGroundCollapseStage1.Spawn(this, Map);
                    }
                }
                else if (CollapseStage == 2)
                {
                    if (collapseSustainer == null)
                    {
                        collapseSustainer = SoundDefOf.PitGateCollapsing.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
                    }
                    collapseSustainer.Maintain();
                    if (collapseEffecter2 == null)
                    {
                        collapseEffecter2 = EffecterDefOf.PitGateAboveGroundCollapseStage2.Spawn(this, Map);
                    }
                    if (Find.CurrentMap == Map && Rand.MTBEventOccurs(2f, 60f, 1f))
                    {
                        Find.CameraDriver.shaker.DoShake(0.2f);
                    }
                }
                collapseEffecter1?.EffectTick(this, this);
                collapseEffecter2?.EffectTick(this, this);

                if (Find.TickManager.TicksGame >= collapseTick)
                {
                    Collapse();
                }
                return;
            }
        }

        private void Collapse()
        {
            collapseSustainer.End();
            collapseEffecter2?.Cleanup();
            collapseEffecter2 = null;
            collapseEffecter1?.Cleanup();
            collapseEffecter1 = null;
            EffecterDefOf.PitGateAboveGroundCollapsed.Spawn(Position, Map);
            if (Find.CurrentMap == kwamaNest)
            {
                SoundDefOf.UndercaveCollapsing_End.PlayOneShotOnCamera();
            }
            else
            {
                SoundDefOf.PitGateCollapsing_End.PlayOneShot(new TargetInfo(Position, Map));
            }
            if (kwamaNest != null)
            {
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Crush, 99999f, 999f);
                for (int num = kwamaNest.mapPawns.AllPawns.Count - 1; num >= 0; num--)
                {
                    Pawn pawn = kwamaNest.mapPawns.AllPawns[num];
                    pawn.TakeDamage(damageInfo);
                    if (!pawn.Dead)
                    {
                        pawn.Kill(damageInfo);
                    }
                }
                PocketMapUtility.DestroyPocketMap(kwamaNest);
            }
            allowDestroyNonDestroyable = true;
            Destroy(DestroyMode.Deconstruct);
            allowDestroyNonDestroyable = false;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode == DestroyMode.Vanish)
            {
                base.Destroy(mode);
                return;
            }
            Map map = Map;
            base.Destroy(mode);
            EffecterDefOf.ImpactDustCloud.Spawn(Position, map).Cleanup();
            Messages.Message("MessagePitGateCollapsed".Translate(), new TargetInfo(Position, map), MessageTypeDefOf.NeutralEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref kwamaNest, "kwamaNestMap");
            Scribe_References.Look(ref nestExit, "kwamaNestExit");
            Scribe_Values.Look(ref collapseTick, "collapseTick", 0);
            Scribe_Values.Look(ref isCollapsing, "isCollapsing", defaultValue: false);
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
                else
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Create inner map",
                        action = delegate
                        {
                            GenerateKwamaNest();
                        }
                    };
                }
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Collapse Kwama Nest Entrance",
                    action = BeginCollapsing
                };
            }
            
        }
    }
}
