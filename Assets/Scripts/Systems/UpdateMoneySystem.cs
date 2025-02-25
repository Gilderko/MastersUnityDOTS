﻿using Components;
using Components.Aspects;
using Components.Spawning;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ProcessProjectileHitsSystem))]
    public partial struct UpdateMoneySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoneyComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moneyStorageEntity = SystemAPI.GetSingletonEntity<MoneyComponent>();
            var moneyStorage = SystemAPI.GetAspect<MoneyStorageAspect>(moneyStorageEntity);
            moneyStorage.EvaluateMoneyBuffer();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}