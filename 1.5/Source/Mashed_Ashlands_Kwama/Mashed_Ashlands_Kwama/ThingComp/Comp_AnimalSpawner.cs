using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class Comp_AnimalSpawner : ThingComp
    {
        public CompProperties_AnimalSpawner Props
        {
            get
            {
                return (CompProperties_AnimalSpawner)props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            for (int i = 0; i < Props.initialSpawnCount; i++)
            {
                SpawnAnimals();
            }
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void CompTickLong()
        {
            if (parent.IsHashIntervalTick(Props.tickInterval))
            {
                SpawnAnimals();
            }
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
                        SpawnAnimals();
                    },
                    defaultLabel = "DEV: Force valid spawn"
                };
            }
        }

        private void SpawnAnimals()
        {
            if (Props.disableIfPolluted && parent.Position.IsPolluted(parent.Map))
            {
                FleckMaker.ThrowDustPuffThick(parent.Position.ToVector3(), parent.Map, 1f, Color.green);
                return;
            }
            UnderBiomeProperties biomeProps = UnderBiomeProperties.Get(parent.Map.Biome);
            if (biomeProps != null && !biomeProps.wildAnimals.NullOrEmpty())
            {
                List<AnimalSpawns> validSpawns = biomeProps.wildAnimals.Where(x => parent.Map.mapPawns.AllPawnsSpawned.Where(y => y.kindDef == x.kindDef).Count() < x.maxOnMap).ToList();
                if (!validSpawns.NullOrEmpty())
                {
                    AnimalSpawns chosenSpawn = validSpawns.RandomElementByWeight(x => x.weight);
                    int count = chosenSpawn.kindDef.wildGroupSize.RandomInRange;
                    for (int i = 0; i < count; i++)
                    {
                        if (CellFinder.TryFindRandomCellNear(parent.Position, parent.Map, 1, (IntVec3 c) => c.Standable(parent.Map), out IntVec3 spawnPoint))
                        {
                            GenSpawn.Spawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenSpawn.kindDef)), spawnPoint, parent.Map, Rot4.Random);
                        }
                        else
                        {
                            GenSpawn.Spawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenSpawn.kindDef)), parent.Position, parent.Map, Rot4.Random);
                        }
                    }
                }
            }
            else
            {
                Log.Warning("UnderBiomeProperties missing or wildAnimals is null in biome def: " + parent.Map.Biome);
            }
        }
    }
}
