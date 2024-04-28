using System.Collections;
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

        public Map SourceMap => (map.Parent as PocketMapParent)?.sourceMap;
        public KwamaNestMapComponent(Map map) : base(map)
        {
        }

        public void Notify_BeginCollapsing()
        {
            SoundDefOf.UndercaveRumble.PlayOneShotOnCamera(map);
            Find.CameraDriver.shaker.DoShake(0.2f, 120);
        }

        public override void MapComponentTick()
        {
            if (nestEntrance == null || Find.CurrentMap != map)
            {
                return;
            }
            if (nestEntrance.IsCollapsing)
            {

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
            List<Pawn> workers = AllOfKind(map, workerKind);
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
            List<Pawn> warriors = AllOfKind(queen, warriorKind);
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
                Log.Message("bruh");
                for (int i = burrows.Count-1; i >= 0; i--)
                {
                    burrows[i].Destroy();
                }
            }
        }

        private List<Pawn> AllOfKind(Thing queen, PawnKindDef kind)
        {
            return AllOfKind(queen.Map, kind);
        }

        private List<Pawn> AllOfKind(Map map, PawnKindDef kind)
        {
            return map.mapPawns.AllPawnsSpawned.Where(x => x.kindDef == kind && x.Faction == null).ToList();
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
