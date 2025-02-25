﻿using Components;
using Components.Enemy;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Jobs
{
    [BurstCompile]
    public partial struct ProjectileCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<LocalTransform> Positions;
        [ReadOnly] public ComponentLookup<ImpactComponent> Projectiles;
        [ReadOnly] public ComponentLookup<ProjectileConfigComponent> ProjectileConfigs;
        [ReadOnly] public ComponentLookup<HealthComponent> Healths;
        
        public EntityCommandBuffer ECB;
        public PhysicsWorldSingleton PhysicsWorld;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            var projectile = Entity.Null;
            var enemy = Entity.Null;

            if (Projectiles.HasComponent(triggerEvent.EntityA))
                projectile = triggerEvent.EntityA;
            if (Projectiles.HasComponent(triggerEvent.EntityB))
                projectile = triggerEvent.EntityB;
            if (Healths.HasComponent(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (Healths.HasComponent(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;

            if (Entity.Null.Equals(projectile)
                || Entity.Null.Equals(enemy))
            {
                return;
            }
            
            var projectileConfig = ProjectileConfigs[projectile].Config.Value;
            var damageToDeal = projectileConfig.Damage;
            var projectilePosition = Positions[projectile].Position;
            if (projectileConfig.ProjectileType == ProjectileType.Explosive)
            {
                var distances = new NativeList<DistanceHit>(Allocator.Temp);
                if (PhysicsWorld.OverlapSphere(projectilePosition, projectileConfig.ExplosiveDamageRadius, ref distances, projectileConfig.CollisionFilter))
                {
                    foreach (var hit in distances)
                    {
                        ECB.AppendToBuffer(hit.Entity, new HitDataComponent()
                        {
                            DamageToTake = damageToDeal
                        });
                    }
                }

                distances.Dispose();
            }
            else
            {
                ECB.AppendToBuffer(enemy, new HitDataComponent()
                {
                    DamageToTake = damageToDeal
                });
            }
            
            var impactPrefab = Projectiles[projectile].VFXImpactPrefab;
            if (impactPrefab != Entity.Null)
            {
                var impactEntity = ECB.Instantiate(impactPrefab);
                ECB.SetComponent(impactEntity, LocalTransform.FromPosition(Positions[enemy].Position + math.up() * 0.5f));
            }
            
            ECB.DestroyEntity(projectile);
        }
    }
}