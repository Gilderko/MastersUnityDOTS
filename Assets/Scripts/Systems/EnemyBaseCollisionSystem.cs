using Components;
using Components.Enemy;
using Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

/// <summary>
/// Scheduled a CollisionJob which processes all collisions between enemies and the friendly base
/// </summary>
[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct EnemyBaseCollisionSystem : ISystem
{
    private ComponentLookup<EnemyConfigComponent> _enemyConfigLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FriendlyBaseTag>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _enemyConfigLookup = SystemAPI.GetComponentLookup<EnemyConfigComponent>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbBos = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        _enemyConfigLookup.Update(ref state);

        var friendlyBaseEntity = SystemAPI.GetSingletonEntity<FriendlyBaseTag>();
        var friendlyBaseHealth = SystemAPI.GetComponent<HealthComponent>(friendlyBaseEntity);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var result = new NativeArray<int>(1, Allocator.TempJob);
        
        var job = new EnemyBaseCollisionJob()
        {
            TotalDamageToDeal = result,
            FriendlyBaseEntity = friendlyBaseEntity,
            EnemyConfigs = _enemyConfigLookup,
            ECB = ecbBos
        };

        var jobHandle = job.Schedule(simulation, state.Dependency);
        jobHandle.Complete();

        var healthToSubtract = result[0];
        if (friendlyBaseHealth.Value < healthToSubtract)
        {
            ecbBos.AddComponent<LevelEndComponent>(friendlyBaseEntity, new LevelEndComponent()
            {
                Success = false
            });
        }
        else
        {
            ecbBos.SetComponent(friendlyBaseEntity, new HealthComponent
            {
                InitialValue = friendlyBaseHealth.InitialValue,
                Value = friendlyBaseHealth.Value - healthToSubtract
            });
        }
        
        result.Dispose();
    }
}