using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace Mashed_Ashlands_Kwama
{
    public class Comp_KwamaQueen : ThingComp
    {
        public CompProperties_KwamaQueen Props
        {
            get
            {
                return (CompProperties_KwamaQueen)props;
            }
        }

        private KwamaNestMapComponent kwamaNestMapComponent;
        public float eggSacProgress;

        private KwamaNestMapComponent NestComp
        {
            get
            {
                if (kwamaNestMapComponent == null)
                {
                    kwamaNestMapComponent = parent.Map.GetComponent<KwamaNestMapComponent>();
                    if (kwamaNestMapComponent == null)
                    {
                        parent.Destroy();
                    }
                }
                return kwamaNestMapComponent;
            }
        }

        private bool Active
        {
            get
            {
                return !parent.Position.Fogged(parent.Map);
            }
        }

        public override void CompTick()
        {
            if (Active)
            {
                float num = 1f / (Props.intervalDays * GenDate.TicksPerDay);
                eggSacProgress += num;
                if (eggSacProgress >= 1f)
                {
                    if (NestComp.EggSacReady(parent, Props.workerKind))
                    {
                        Props.sacSpawnSound.PlayOneShot(parent);
                        eggSacProgress = 0f;
                    }
                    else
                    {
                        SpawnSacNear();
                    }
                    eggSacProgress = 0f;
                }
            }
        }

        public void SpawnSacNear()
        {
            if (CellFinder.TryFindRandomCellNear(parent.Position, parent.Map, 5, (IntVec3 c) => c.Standable(parent.Map) && !c.GetTerrain(parent.Map).IsWater, out IntVec3 eggSacSpawn))
            {
                Props.sacSpawnSound.PlayOneShot(parent);
                GenSpawn.Spawn(KwamaUtility.GetKwamaEggType(), eggSacSpawn, parent.Map);
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            NestComp.QueenDamaged(parent, Props.warriorKind);
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            KwamaNestMapComponent prevNestComp = prevMap.GetComponent<KwamaNestMapComponent>();
            if (prevNestComp != null)
            {
                prevNestComp.QueenKilled(prevMap, Props.workerKind);
                Find.LetterStack.ReceiveLetter("Mashed_Ashlands_Kwama_NestCollapse_Label".Translate(), "Mashed_Ashlands_Kwama_NestCollapse_Description".Translate(), LetterDefOf.NegativeEvent);
            }
            base.Notify_Killed(prevMap, dinfo);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref eggSacProgress, "eggSacProgress", 0f);
        }

        public override string CompInspectStringExtra()
        {
            return "Mashed_Ashlands_Kwama_EggSacProgress".Translate(eggSacProgress.ToStringPercent());
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    action = delegate
                    {
                        eggSacProgress = 1f;
                    },
                    defaultLabel = "DEV: Max egg sac progress"
                };
            }
        }
    }
}
