using Components;
using Components.Aspects;
using Components.Spawning;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    /// <summary>
    /// Queries the Spawner aspect to find if the final wave is finished and all enemies have been killed
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct CheckGameStateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SpawnerTagComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbBos = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            var waveAspectEntity = SystemAPI.GetSingletonEntity<SpawnerTagComponent>();
            var waveAspect = SystemAPI.GetAspect<SpawnerAspect>(waveAspectEntity);

            if (!waveAspect.IsLevelFinished)
            {
                return;
            }
            
            ecbBos.AddComponent(waveAspectEntity, new LevelEndComponent()
            {
                Success = true
            });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}