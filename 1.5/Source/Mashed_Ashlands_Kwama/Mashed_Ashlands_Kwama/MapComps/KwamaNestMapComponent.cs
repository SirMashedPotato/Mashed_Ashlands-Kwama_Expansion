using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class KwamaNestMapComponent : CustomMapComponent
    {
        public KwamaNestMapComponent(Map map) : base(map)
        {
        }

        public bool EggSacReady(Thing queen)
        {

            return false;
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

        public bool QueenKilled(Map map, PawnKindDef workerKind)
        {
            PanicWorkers(map, workerKind);
            return true;
        }

        private void PanicWorkers(Map map, PawnKindDef workerKind)
        {
            List<Pawn> workers = AllOfKind(map, workerKind);
            if (!workers.NullOrEmpty())
            {
                TriggerMentalState(workers, MentalStateDefOf.PanicFlee);
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
