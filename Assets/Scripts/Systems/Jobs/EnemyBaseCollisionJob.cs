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
    public partial struct EnemyBaseCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public Entity FriendlyBaseEntity;
        [ReadOnly] public ComponentLookup<EnemyConfigComponent> EnemyConfigs;
        
        public EntityCommandBuffer ECB;
        public NativeArray<int> TotalDamageToDeal;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            var friendlyBase = Entity.Null;
            var enemy = Entity.Null;

            if (FriendlyBaseEntity == triggerEvent.EntityA)
                friendlyBase = triggerEvent.EntityA;
            if (FriendlyBaseEntity == triggerEvent.EntityB)
                friendlyBase = triggerEvent.EntityB;
            if (EnemyConfigs.HasComponent(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (EnemyConfigs.HasComponent(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;

            if (Entity.Null.Equals(friendlyBase)
                || Entity.Null.Equals(enemy))
            {
                return;
            }
            
            var projectileConfig = EnemyConfigs[enemy].Config.Value;
            TotalDamageToDeal[0] += projectileConfig.AttackDamage;
            
            ECB.DestroyEntity(enemy);
        }
    }
}