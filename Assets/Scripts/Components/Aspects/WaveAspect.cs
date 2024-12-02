using System.Linq;
using Components.Enemy;
using Components.Shared;
using Components.Spawning;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct WaveAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<WaveInfoComponent> _waveInfo;
        private readonly RefRO<PathComponent> _path;
        private readonly RefRW<TimerComponent> _timer;
        private readonly RefRW<RandomComponent> _random;
        
        private readonly DynamicBuffer<EntitySpawnData> _entitiesToSpawn;

        public void SpawnNewEntity(float deltaTime, EntityCommandBuffer ecb, SpawnerAspect spawner)
        {
            _timer.ValueRW.TimerValue -= deltaTime;
            
            if (_timer.ValueRO.TimerValue > 0)
            {
                return;
            }

            if (_entitiesToSpawn.Length == 0)
            {
                spawner.MoveToNextWave(Entity, ecb);
                return;
            }
            
            var nextEntityIndex = _random.ValueRW.RandomValue.NextInt(0, _entitiesToSpawn.Length - 1);
            var entityToSpawn = _entitiesToSpawn[nextEntityIndex];

            if (entityToSpawn.VFXToSpawn != Entity.Null)
            {
                var vfxEntity = ecb.Instantiate(entityToSpawn.VFXToSpawn);
                ecb.AddComponent(vfxEntity, new LocalTransform()
                {   
                    Position = _path.ValueRO.Path.Value.Waypoints[0] + entityToSpawn.ParticleSystemSpawnModifier.PositionSpawnOffset,
                    Rotation = quaternion.EulerXYZ(entityToSpawn.ParticleSystemSpawnModifier.RotationSpawnOffset),
                    Scale = entityToSpawn.ParticleSystemSpawnModifier.ScaleMultiplication,
                });
            }
            
            var enemyEntity = ecb.Instantiate(entityToSpawn.EntityToSpawn);
            ecb.AddComponent(enemyEntity, new LocalTransform()
            {
                Position = _path.ValueRO.Path.Value.Waypoints[0],
                Rotation = quaternion.identity,
                Scale = 1,
            });
            ecb.AddComponent(enemyEntity, new PathComponent()
            {
                Path = _path.ValueRO.Path,
            });
            ecb.AddComponent(enemyEntity, new NextPathIndexComponent()
            {
                NextIndex = 1,
            });
            
            _entitiesToSpawn.RemoveAtSwapBack(nextEntityIndex);
            entityToSpawn.CountToSpawn -= 1;
            if (entityToSpawn.CountToSpawn > 0)
            {
                _entitiesToSpawn.Add(entityToSpawn);
            }
            
            _timer.ValueRW.TimerValue = _waveInfo.ValueRO.TimeBetweenSpawns;
        }
    }
}