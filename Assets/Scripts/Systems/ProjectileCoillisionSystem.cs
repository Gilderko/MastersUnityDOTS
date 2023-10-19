using Components;
using Components.Enemy;
using Systems.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct ProjectileCoillisionSystem : ISystem
{
    private ComponentLookup<LocalTransform> _positionLookup;
    private ComponentLookup<ImpactComponent> _impactLookup;
    private ComponentLookup<HealthComponent> _healthLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        _impactLookup = SystemAPI.GetComponentLookup<ImpactComponent>(true);
        _healthLookup = SystemAPI.GetComponentLookup<HealthComponent>(false);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbBos = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        _positionLookup.Update(ref state);
        _healthLookup.Update(ref state);
        _impactLookup.Update(ref state);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        
        var job = new ProjectileCollisionJob()
        {
            Projectiles = _impactLookup,
            EnemiesHealth = _healthLookup,
            Positions = _positionLookup,
            ECB = ecbBos
        };

        state.Dependency = job.Schedule(simulation, state.Dependency);
    }
}