﻿using Components.Shared;
using Components.Spawning;
using Unity.Entities;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct SpawnerAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<SpawnerTagComponent> _spawnerTag;
        private readonly RefRW<LocalTransform> _transform;
        private readonly DynamicBuffer<EntityReferenceBufferElement> _waves;

        public bool IsLevelFinished => _spawnerTag.ValueRO.TotalEnemies == _spawnerTag.ValueRO.TotalEnemiesKilled;
        
        public void SetKilled(int killedAmmount)
        {
            _spawnerTag.ValueRW.TotalEnemiesKilled += killedAmmount;
        }
        
        public void MoveToNextWave(Entity currentWave, EntityCommandBuffer ecb)
        {
            for (var i = 0; i < _waves.Length; i++)
            {
                if (_waves[i].EntityRef != currentWave || i >= _waves.Length - 1)
                {
                    continue;
                }
                
                ecb.SetComponentEnabled<WaveInfoComponent>(currentWave, false);
                ecb.SetComponentEnabled<WaveInfoComponent>(_waves[i+1].EntityRef, true);
            }
        }
    }
}