using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Mashed_Ashlands_Kwama
{
    public class KwamaNestMapComponent : CustomMapComponent
    {
        public KwamaNestEntrance nestEntrance;
        public KwamaNestExit nestExit;

        //Collapsing
        private static readonly IntRange NumRockCollapsesRange = new IntRange(0, 2);

        private static readonly IntRange CollapsedRocksSizeRange = new IntRange(1, 5);

        private const float CollapsedRocksMinDistanceFromColonist = 10f;

        private const int InitialShakeDurationTicks = 120;

        private const float InitialShakeAmount = 0.2f;

        private const float StageOneShakeAmount = 0.1f;

        private const float StageTwoShakeAmount = 0.2f;

        private static readonly IntRange StageOneNumCollapseEffects = new IntRange(10, 15);

        private static readonly IntRange StageTwoNumCollapseEffects = new IntRange(15, 20);

        private static readonly IntRange FXTriggerDelay = new IntRange(10, 30);

        private static readonly SimpleCurve HoursToShakeMTBTicksCurve = new SimpleCurve
        {
            new CurvePoint(14f, 2500f),
            new CurvePoint(1f, 45f)
        };

        private const float AmbientFXMTB = 60f;

        private Sustainer collapsingSustainer;

        private readonly Queue<QueuedCellEffecter> fxQueue = new Queue<QueuedCellEffecter>();

        public Map SourceMap => (map.Parent as PocketMapParent)?.sourceMap;

        public KwamaNestMapComponent(Map map) : base(map)
        {
            UnderBiomeProperties biomeProperties = UnderBiomeProperties.Get(map.Biome);
            if (biomeProperties == null || biomeProperties.forcedCondition == null 
                || map.gameConditionManager.ConditionIsActive(biomeProperties.forcedCondition))
            {
                return;
            }
            GameCondition gameCondition = GameConditionMaker.MakeCondition(biomeProperties.forcedCondition);
            gameCondition.Permanent = true;
            map.gameConditionManager.RegisterCondition(gameCondition);
        }

        public void Notify_BeginCollapsing()
        {
            RimWorld.SoundDefOf.UndercaveRumble.PlayOneShotOnCamera(map);
            Find.CameraDriver.shaker.DoShake(InitialShakeAmount, InitialShakeDurationTicks);
        }

        public override void MapComponentTick()
        {
            if (Find.CurrentMap != map)
            {
                collapsingSustainer?.End();
            }
            if (nestEntrance == null || Find.CurrentMap != map)
            {
                return;
            }
            if (nestEntrance.IsCollapsing)
            {
                float mtb = HoursToShakeMTBTicksCurve.Evaluate(nestEntrance.TicksUntilCollapse / 2500f);
                if (nestEntrance.CollapseStage == 1)
                {
                    if (collapsingSustainer == null || collapsingSustainer.Ended)
                    {
                        collapsingSustainer = RimWorld.SoundDefOf.UndercaveCollapsingStage1.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick));
                    }
                    if (Find.CurrentMap == map && Rand.MTBEventOccurs(mtb, 1f, 1f))
                    {
                        TriggerCollapseFX(StageOneShakeAmount, StageOneNumCollapseEffects.RandomInRange);
                    }
                }
                else
                {
                    if (collapsingSustainer == null || collapsingSustainer.Ended)
                    {
                        collapsingSustainer = RimWorld.SoundDefOf.UndercaveCollapsingStage2.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick));
                    }
                    if (Find.CurrentMap == map && Rand.MTBEventOccurs(mtb, 1f, 1f))
                    {
                        TriggerCollapseFX(StageTwoShakeAmount, StageTwoNumCollapseEffects.RandomInRange);
                    }
                }
                collapsingSustainer.Maintain();
            }
            TriggerAmbientFX();
            ReadFxQueue();
        }

        private void TriggerAmbientFX()
        {
            if (!Rand.MTBEventOccurs(AmbientFXMTB, 1f, 1f))
            {
                return;
            }
            if (Find.CameraDriver.ZoomRootSize < 20f && CellFinderLoose.TryGetRandomCellWith((IntVec3 c) => c.Standable(map) && Find.CameraDriver.CurrentViewRect.Contains(c), map, 100, out var result))
            {
                fxQueue.Enqueue(new QueuedCellEffecter(RimWorld.EffecterDefOf.UndercaveMapAmbienceWater, result, 0));
            }
        }

        private void TriggerCollapseFX(float shakeAmt, int numDustEffecters)
        {
            Find.CameraDriver.shaker.DoShake(shakeAmt);
            RimWorld.SoundDefOf.UndercaveRumble.PlayOneShotOnCamera(map);
            int randomInRange = NumRockCollapsesRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                if (!CellFinderLoose.TryGetRandomCellWith(delegate (IntVec3 c)
                {
                    if (c.GetEdifice(map) != null)
                    {
                        return false;
                    }
                    foreach (Pawn item in map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
                    {
                        if (item.Position.InHorDistOf(c, CollapsedRocksMinDistanceFromColonist))
                        {
                            return false;
                        }
                    }
                    return true;
                }, map, 100, out var result))
                {
                    continue;
                }
                foreach (IntVec3 item2 in GridShapeMaker.IrregularLump(result, map, CollapsedRocksSizeRange.RandomInRange, (IntVec3 c) => c.GetEdifice(map) == null))
                {
                    RoofCollapserImmediate.DropRoofInCells(item2, map);
                }
                RimWorld.EffecterDefOf.UndercaveCeilingDebris.SpawnMaintained(result, map);
            }
            int num = Find.TickManager.TicksGame;
            for (int j = 0; j < numDustEffecters; j++)
            {
                if (CellFinderLoose.TryGetRandomCellWith((IntVec3 c) => c.GetEdifice(map) == null, map, 100, out var result2))
                {
                    fxQueue.Enqueue(new QueuedCellEffecter(RimWorld.EffecterDefOf.UndercaveCeilingDebris, result2, num));
                    num += FXTriggerDelay.RandomInRange;
                }
            }
        }

        public void ReadFxQueue()
        {
            while (fxQueue.Count > 0 && Find.TickManager.TicksGame >= fxQueue.Peek().tick)
            {
                QueuedCellEffecter queuedCellEffecter = fxQueue.Dequeue();
                queuedCellEffecter.effecterDef.SpawnMaintained(queuedCellEffecter.cell, map);
            }
        }

        public override void MapGenerated()
        {
            nestEntrance = SourceMap?.listerThings?.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestEntrance).FirstOrDefault() as KwamaNestEntrance;
            nestExit = map.listerThings.ThingsOfDef(ThingDefOf.Mashed_Ashlands_KwamaNestExit).FirstOrDefault() as KwamaNestExit;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref nestEntrance, "nestEntrance");
            Scribe_References.Look(ref nestExit, "nestExit");
        }

        public bool EggSacReady(Thing queen, PawnKindDef workerKind)
        {
            List<Pawn> workers = KwamaUtility.AllOfKind(map, workerKind);
            if (!workers.NullOrEmpty())
            {
                foreach (Pawn potentialWorker in workers.InRandomOrder())
                {
                    if (potentialWorker.CanReach(queen, PathEndMode.Touch, Danger.Deadly, true))
                    {
                        IntVec3 placeCell = FindEggSacPlacementCell(potentialWorker);
                        if (placeCell != null)
                        {
                            Job workerJob = JobMaker.MakeJob(JobDefOf.Mashed_Ashlands_MoveEggSac, queen, placeCell);
                            potentialWorker.jobs.StartJob(workerJob, JobCondition.InterruptForced);
                            return true;
                        }
                    }
                    
                }
            }
            return false;
        }

        private IntVec3 FindEggSacPlacementCell(Pawn worker)
        {
            return CellFinder.RandomClosewalkCellNear(worker.Position, worker.Map, 6);
        }

        public bool QueenDamaged(Thing queen, PawnKindDef warriorKind)
        {
            List<Pawn> warriors = KwamaUtility.AllOfKind(queen, warriorKind);
            if (!warriors.NullOrEmpty())
            {
                List<Pawn> manhunteredPawns = TriggerMentalState(warriors, MentalStateDefOf.Manhunter);
                if (!manhunteredPawns.NullOrEmpty())
                {
                    Find.LetterStack.ReceiveLetter("Mashed_Ashlands_Kwama_QueenDamaged_Label".Translate(queen.def.label),
                        "Mashed_Ashlands_Kwama_QueenDamaged_Description".Translate(queen.def.label, warriorKind.label),
                        (manhunteredPawns.Count == 1) ? LetterDefOf.ThreatSmall : LetterDefOf.ThreatBig, queen);
                }
                return true;
            }
            return false;
        }

        public bool QueenKilled()
        {
            PanicWildAnimals();
            DestroyAllBurrows();
            nestEntrance.BeginCollapsing();
            return true;
        }

        private void PanicWildAnimals()
        {
            List<Pawn> workers = map.mapPawns.AllPawnsSpawned.Where(x => x.RaceProps.Animal && x.Faction == null).ToList();
            if (!workers.NullOrEmpty())
            {
                TriggerMentalState(workers, MentalStateDefOf.PanicFlee);
            }
        }

        private void DestroyAllBurrows()
        {
            List<Building> burrows = map.listerBuildings.AllBuildingsNonColonistOfDef(ThingDefOf.Mashed_Ashlands_KwamaBurrow).ToList();
            if (!burrows.NullOrEmpty())
            {
                for (int i = burrows.Count-1; i >= 0; i--)
                {
                    burrows[i].Destroy();
                }
            }
        }

        private List<Pawn> TriggerMentalState(List<Pawn> pawnList, MentalStateDef stateDef)
        {
            List<Pawn> affectedPawns = new List<Pawn>();
            foreach (Pawn pawn in pawnList)
            {
                if (pawn.mindState.mentalStateHandler.TryStartMentalState(stateDef: stateDef, forced: true, forceWake: true))
                {
                    affectedPawns.Add(pawn);
                }
            }
            return affectedPawns ?? null;
        }
    }
}
