using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace Mashed_Ashlands_Kwama
{
    public class KwamaNestMapComponent : CustomMapComponent
    {
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
                        if (placeCell != IntVec3.Invalid && placeCell != potentialWorker.Position)
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
            return CellFinder.RandomClosewalkCellNear(worker.Position, worker.Map, 6, (IntVec3 x) => x.InBounds(map) && x.GetFirstBuilding(map) == null);
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
            //todo add permanent unstable tunnels condition
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
