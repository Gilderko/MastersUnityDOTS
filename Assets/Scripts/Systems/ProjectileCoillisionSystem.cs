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
    private ComponentLookup<ProjectileConfigComponent> _projectileConfigLookup;
    private ComponentLookup<HealthComponent> _healthLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        _impactLookup = SystemAPI.GetComponentLookup<ImpactComponent>(true);
        _projectileConfigLookup = SystemAPI.GetComponentLookup<ProjectileConfigComponent>(true);
        _healthLookup = SystemAPI.GetComponentLookup<HealthComponent>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbBos = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        _positionLookup.Update(ref state);
        _projectileConfigLookup.Update(ref state);
        _impactLookup.Update(ref state);
        _healthLookup.Update(ref state);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        
        var job = new ProjectileCollisionJob()
        {
            Projectiles = _impactLookup,
            ProjectileConfigs = _projectileConfigLookup,
            Positions = _positionLookup,
            Healths = _healthLookup,
            ECB = ecbBos
        };

        state.Dependency = job.Schedule(simulation, state.Dependency);
    }
}