using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class JobDriver_MoveEggSac : JobDriver
    {
        protected Pawn Queen => job.targetA.Pawn;
        protected IntVec3 TargetCell => job.targetB.Cell;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job) && pawn.Reserve(job.GetTarget(TargetIndex.B), job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            ///Kwama queen
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.A, DurationTicks, true, true);
            ///Placement location
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.B, DurationTicks, true, true);
            yield return Toils_General.Do(delegate
            {
                GenSpawn.Spawn(KwamaUtility.GetKwamaEggType(), TargetCell, pawn.Map);
            });
            yield break;
        }

        private const int DurationTicks = 100;
    }
}
