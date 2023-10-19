using Components;
using Components.Enemy;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Jobs
{
    [BurstCompile]
    public partial struct ProjectileCollisionJob : ITriggerEventsJob
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> Positions;
        [ReadOnly]
        public ComponentLookup<ImpactComponent> Projectiles;
        public ComponentLookup<HealthComponent> EnemiesHealth;

        public EntityCommandBuffer ECB;

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
            if (EnemiesHealth.HasComponent(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (EnemiesHealth.HasComponent(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;

            // if its a pair of entity we don't want to process, exit
            if (Entity.Null.Equals(projectile)
                || Entity.Null.Equals(enemy))
            {
                return;
            }

            // Damage enemy
            var hp = EnemiesHealth[enemy];
            hp.Value -= 5;
            EnemiesHealth[enemy] = hp;

            if (hp.Value <= 0)
            {
                ECB.DestroyEntity(enemy);
            }

            // Spawn VFX
            var impactEntity = ECB.Instantiate(Projectiles[projectile].VFXImpactPrefab);
            ECB.SetComponent(impactEntity, LocalTransform.FromPosition(Positions[enemy].Position));
            
            ECB.DestroyEntity(projectile);
        }
    }
}