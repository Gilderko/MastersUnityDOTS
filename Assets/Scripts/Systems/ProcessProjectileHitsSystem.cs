using Components;
using Systems.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ProjectileCoillisionSystem))]
    public partial struct ProcessProjectileHitsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoneyComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var moneyStorageEntity = SystemAPI.GetSingletonEntity<MoneyComponent>();
            
            var hitsJob = new ProcessProjectileHitsJob()
            {
                ECB = ecb,
                MoneyStorageEntity = moneyStorageEntity,
            };

            hitsJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}