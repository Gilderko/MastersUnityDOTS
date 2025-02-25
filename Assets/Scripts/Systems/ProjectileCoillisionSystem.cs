﻿using Components;
using Components.Enemy;
using Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

/// <summary>
/// Schedules ProjectileCollisionJob which filters through all collisions and generates collision data 
/// </summary>
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
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<SimulationSingleton>();
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
        var ecbBos = new EntityCommandBuffer(Allocator.TempJob);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        
        _positionLookup.Update(ref state);
        _projectileConfigLookup.Update(ref state);
        _impactLookup.Update(ref state);
        _healthLookup.Update(ref state);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        
        var job = new ProjectileCollisionJob()
        {
            PhysicsWorld = physicsWorld,
            Projectiles = _impactLookup,
            ProjectileConfigs = _projectileConfigLookup,
            Positions = _positionLookup,
            Healths = _healthLookup,
            ECB = ecbBos
        };
        
        var jobHandle = job.Schedule(simulation, state.Dependency);
        jobHandle.Complete();
        
        ecbBos.Playback(state.EntityManager);
        ecbBos.Dispose();
    }
}