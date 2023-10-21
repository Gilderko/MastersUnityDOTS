using System.Collections.Generic;
using System.Linq;
using Components;
using Components.Shared;
using Components.Spawning;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityMonoBehaviour.SpawningConfig;
using Utils;

namespace Authoring
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public List<Transform> EnemyPath = new List<Transform>();
        public LevelSpawnInfo LevelInfo;
        
        private class SpawnerAuthoringBaker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var spawnerEntity = GetEntity(TransformUsageFlags.Dynamic);

                var waveBuffer = AddBuffer<EntityReferenceBufferElement>(spawnerEntity);
                
                var blobPathReference = GeneratePaths(authoring);
                foreach (var (newWave, index) in authoring.LevelInfo.Waves.WithIndex())
                {
                    PrepareWaves(waveBuffer, blobPathReference, newWave, index);
                }
                
                AddComponent(spawnerEntity, new SpawnerTagComponent());
            }

            private void PrepareWaves(DynamicBuffer<EntityReferenceBufferElement> waveBuffer, BlobAssetReference<BlobPath> blobPathReference, WaveInfo newWave, int index)
            {
                var newWaveEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                waveBuffer.Add(new EntityReferenceBufferElement()
                {
                    EntityRef = newWaveEntity
                });

                AddComponent(newWaveEntity, new PathComponent() { Path = blobPathReference });
                AddComponent(newWaveEntity, new WaveInfoComponent()
                {
                    AlreadySpawnedCount = 0,
                    MaxSpawnedAtOnce = newWave.MaxSpawnedAtOnce,
                    TimeBetweenSpawns = newWave.TimeBetweenSpawns
                });
                SetComponentEnabled<WaveInfoComponent>(newWaveEntity, index == 0);
                AddComponent(newWaveEntity, new RandomComponent()
                {
                    RandomValue = new Unity.Mathematics.Random(5)
                });
                AddComponent(newWaveEntity, new TimerComponent()
                {
                    TimerValue = 0
                });

                var enemiesBuffer = AddBuffer<EntityAndProbabilityData>(newWaveEntity);

                foreach (var enemyInfo in newWave.EntitiesToSpawn)
                {
                    enemiesBuffer.Add(new EntityAndProbabilityData()
                    {
                        CountToSpawn = enemyInfo.CountEntitiesToSpawn,
                        EntityToSpawn = GetEntity(enemyInfo.SpawnableEntity, TransformUsageFlags.Dynamic),
                        VFXToSpawn = GetEntity(enemyInfo.VFXToInstantWhenSpawned, TransformUsageFlags.Dynamic),
                        ParticleSystemSpawnModifier = new SpawnTransformModifierData()
                        {
                            PositionSpawnOffset = enemyInfo.ParticleSpawnTransformModifier.PositionSpawnOffset,
                            RotationSpawnOffset = enemyInfo.ParticleSpawnTransformModifier.RotationOffset,
                            ScaleMultiplication = enemyInfo.ParticleSpawnTransformModifier.ScaleMultiplication,
                        }
                    });
                }
            }

            private BlobAssetReference<BlobPath> GeneratePaths(SpawnerAuthoring authoring)
            {
                BlobAssetReference<BlobPath> blobPathReference;
                using (var bb = new BlobBuilder(Allocator.Temp))
                {
                    ref var blobPath = ref bb.ConstructRoot<BlobPath>();

                    var waypoints = bb.Allocate(ref blobPath.Waypoints, authoring.EnemyPath.Count);
                    for (var i = 0; i < authoring.EnemyPath.Count; i++)
                    {
                        waypoints[i] = authoring.EnemyPath.ElementAt(i).position;
                    }

                    blobPathReference = bb.CreateBlobAssetReference<BlobPath>(Allocator.Persistent);
                }

                AddBlobAsset(ref blobPathReference, out var _);
                return blobPathReference;
            }
        }
    }
}