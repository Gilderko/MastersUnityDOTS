﻿using System;
using Components;
using Components.Aspects;
using Components.Spawning;
using Systems.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ProjectileCoillisionSystem))]
    public partial struct ProcessProjectileHitsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerTagComponent>();
            state.RequireForUpdate<MoneyComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var moneyStorageEntity = SystemAPI.GetSingletonEntity<AddMoneyElement>();

            var entitiesKilled = 0;
            foreach (var enemyAspect in SystemAPI.Query<EnemyAspect>())
            {
                enemyAspect.EvaluateHealthBuffer();

                if (enemyAspect.CurrentHealth > 0)
                {
                    continue;
                }

                entitiesKilled++;
                ecb.AppendToBuffer(moneyStorageEntity, new AddMoneyElement()
                {
                    MoneyReward = enemyAspect.EnemyConfig.Reward
                });
                
                ecb.DestroyEntity(enemyAspect.Entity);
            }
            
            var waveAspectEntity = SystemAPI.GetSingletonEntity<SpawnerTagComponent>();
            var waveAspect = SystemAPI.GetAspect<SpawnerAspect>(waveAspectEntity);
            
            waveAspect.SetKilled(entitiesKilled);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}