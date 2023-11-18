using Components;
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

            // Identify which entity is which
            if (Projectiles.HasComponent(triggerEvent.EntityA))
                projectile = triggerEvent.EntityA;
            if (Projectiles.HasComponent(triggerEvent.EntityB))
                projectile = triggerEvent.EntityB;
            if (Healths.HasComponent(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (Healths.HasComponent(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;

            // if its a pair of entity we don't want to process, exit
            if (Entity.Null.Equals(projectile)
                || Entity.Null.Equals(enemy))
            {
                return;
            }
            
            // Damage enemy
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
            
            // Spawn VFX
            var impactEntity = ECB.Instantiate(Projectiles[projectile].VFXImpactPrefab);
            ECB.SetComponent(impactEntity, LocalTransform.FromPosition(Positions[enemy].Position));
            
            ECB.DestroyEntity(projectile);
        }
    }
}