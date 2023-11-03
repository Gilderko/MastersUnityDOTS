using Components;
using Components.Aspects;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Jobs
{
    [BurstCompile]
    public partial struct ProcessProjectileHitsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity MoneyStorageEntity;
        
        [BurstCompile]
        public void Execute(EnemyAspect enemyAspect, [EntityIndexInQuery] int sortKey)
        {
            enemyAspect.EvaluateHealthBuffer();

            if (enemyAspect.CurrentHealth > 0)
            {
                return;
            }
           
            ECB.AppendToBuffer(sortKey, MoneyStorageEntity, new AddMoneyElement()
            {
                MoneyReward = enemyAspect.EnemyConfig.Reward
            });
            ECB.DestroyEntity(sortKey, enemyAspect.Entity);
        }
    }
}