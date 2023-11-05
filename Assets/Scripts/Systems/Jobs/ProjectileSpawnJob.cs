using Components;
using Components.Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Jobs
{
    [BurstCompile]
    public partial struct ProjectileSpawnJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton PhysicsWorld;
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        public void Execute(TowerAspect towerAspect, [EntityIndexInQuery] int sortKey)
        {
            towerAspect.DecrementProjectileTimer(DeltaTime);
            if (towerAspect.ProjectileTimerComponent.TimerValue > 0)
            {
                return;
            }

            ref var towerConfig = ref towerAspect.TowerConfig.Config.Value;
            var closestHitCollector = new ClosestHitCollector<DistanceHit>(towerConfig.Range);

            if (!PhysicsWorld.OverlapSphereCustom(towerAspect.Transform.Position, towerConfig.Range, ref closestHitCollector, towerConfig.Filter))
            {
                return;
            }

            towerAspect.ResetProjectileTimer();

            var entity = ECB.Instantiate(sortKey, towerAspect.TowerData.ProjectilePrefab);
            var transformComponent =
                LocalTransform.FromMatrix(float4x4.LookAt(towerAspect.Transform.Position, closestHitCollector.ClosestHit.Position, towerAspect.Transform.Up()));
            transformComponent.Position = towerAspect.Transform.Position;
            ECB.SetComponent(sortKey, entity, transformComponent);
            ECB.AddComponent(sortKey, entity, new TargetComponent() { Value = closestHitCollector.ClosestHit.Entity });
        }
    }
}