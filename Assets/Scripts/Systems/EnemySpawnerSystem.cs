using Components.Aspects;
using Components.Spawning;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    public partial struct EnemySpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerTagComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            var entity = SystemAPI.GetSingletonEntity<SpawnerTagComponent>();
            var spawnerAspect = SystemAPI.GetAspect<SpawnerAspect>(entity);
            
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var wave in SystemAPI.Query<WaveAspect>())
            {
                wave.SpawnNewEntity(deltaTime, ecb, spawnerAspect);
            }
        }
    }
}