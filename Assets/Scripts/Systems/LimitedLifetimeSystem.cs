using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;

namespace Systems
{
    [BurstCompile]
    public partial struct LimitedLifetimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbBos = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (time,entity) 
                     in SystemAPI.Query<RefRW<LimitedLifeComponent>>().WithEntityAccess())
            {
                time.ValueRW.LifeRemainingSeconds -= SystemAPI.Time.DeltaTime;
                if(time.ValueRO.LifeRemainingSeconds < 0)
                {
                    ecbBos.DestroyEntity(entity);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}