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
        private static readonly CachedTexture EnterPitGateTex = new CachedTexture("UI/Commands/Mashed_Ashlands_EnterUndermap");
        private static readonly CachedTexture ViewUndercaveTex = new CachedTexture("UI/Commands/Mashed_Ashlands_ViewUndermap");
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

        private List<FloatMenuOption> debugGenOptions;

        public List<FloatMenuOption> DebugGenOptions
        {
            get
            {
                if (debugGenOptions.NullOrEmpty())
                {
                    debugGenOptions = new List<FloatMenuOption>();
                    RandomMapGenDef randomMapGenDef = RandomMapGenDef.Get(def);
                    foreach (MapGeneratorDef mapGeneratorDef in randomMapGenDef.mapGenerators)
                    {
                        FloatMenuOption item = new FloatMenuOption(mapGeneratorDef.LabelCap, delegate
                        {
                            GenerateKwamaNest(mapGeneratorDef);
                        });
                        debugGenOptions.Add(item);
                    }
                }
                return debugGenOptions;
            }
        }

        public void GenerateKwamaNest(MapGeneratorDef forcedMapGenerator = null)
        {
            RandomMapGenDef randomMapGenDef = RandomMapGenDef.Get(def);
            kwamaNest = PocketMapUtility.GeneratePocketMap(new IntVec3(kwamaNestSize, 1, kwamaNestSize), 
                forcedMapGenerator ?? randomMapGenDef.mapGenerators.RandomElement(), null, Map);
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
                        collapseEffecter1 = EffecterDefOf.Mashed_Ashlands_KwamaNestAboveGroundCollapseStage1.Spawn(this, Map);
                    }
                }
                else if (CollapseStage == 2)
                {
                    if (collapseSustainer == null)
                    {
                        collapseSustainer = RimWorld.SoundDefOf.PitGateCollapsing.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
                    }
                    collapseSustainer.Maintain();
                    if (collapseEffecter2 == null)
                    {
                        collapseEffecter2 = EffecterDefOf.Mashed_Ashlands_KwamaNestAboveGroundCollapseStage2.Spawn(this, Map);
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
            SoundDefOf.BuildingDestroyed_Stone_Big.PlayOneShot(new TargetInfo(Position, Map));
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
            EffecterDefOf.Mashed_Ashlands_KwamaNestCollapseDustCloud.Spawn(Position, map).Cleanup();
            Messages.Message("Mashed_Ashlands_Kwama_NestCollapsed".Translate(), new TargetInfo(Position, map), MessageTypeDefOf.NeutralEvent);
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
                    defaultLabel = "Mashed_Ashlands_Kwama_ViewNest_Label".Translate(),
                    defaultDesc = "Mashed_Ashlands_Kwama_ViewNest_Description".Translate(),
                    icon = ViewUndercaveTex.Texture,
                    action = delegate
                    {
                        CameraJumper.TryJumpAndSelect(nestExit);
                    }
                };
            }
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Collapse Kwama Nest Entrance",
                    action = BeginCollapsing
                };
                if (kwamaNest == null)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Create inner map",
                        action = delegate
                        {
                            FloatMenu floatMenu = new FloatMenu(DebugGenOptions);
                            Find.WindowStack.Add(floatMenu);
                        }
                    };
                }
            }
        }
    }
}
