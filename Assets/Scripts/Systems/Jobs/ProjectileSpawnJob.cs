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
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] 
        public ComponentLookup<LocalToWorld> WorldLookup;
        
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        public void Execute(TowerAspect towerAspect, [EntityIndexInQuery] int sortKey)
        {
            towerAspect.DecrementProjectileTimer(DeltaTime);
            
            var towerTransformComponent = TransformLookup[towerAspect.Entity];
            var towerPosition = towerTransformComponent.Position;
            var towerConfig = towerAspect.TowerConfig.Config.Value;
            var closestHitCollector = new ClosestHitCollector<DistanceHit>(towerConfig.FireRange);

            if (!PhysicsWorld.OverlapSphereCustom(towerPosition, towerConfig.FireRange, ref closestHitCollector, towerConfig.Filter))
            {
                return;
            }
            
            var lookRotation = quaternion.LookRotationSafe(closestHitCollector.ClosestHit.Position - towerPosition, math.up());
            var currentTowerHeadPosition = TransformLookup[towerAspect.TowerHead.TowerHead];
            currentTowerHeadPosition.Rotation = lookRotation;
            TransformLookup[towerAspect.TowerHead.TowerHead] = currentTowerHeadPosition;
            
            if (towerAspect.ProjectileTimerComponent.TimerValue > 0)
            {
                return;
            }

            var headWorldPos = WorldLookup[towerAspect.TowerHead.TowerHead];
            var spawnPos = headWorldPos.Position + currentTowerHeadPosition.Up() + currentTowerHeadPosition.Forward();
            towerAspect.ResetProjectileTimer();
            
            var entity = ECB.Instantiate(sortKey, towerAspect.TowerData.ProjectilePrefab);
            var transformComponent =
                LocalTransform.FromMatrix(float4x4.LookAt(spawnPos, closestHitCollector.ClosestHit.Position, towerTransformComponent.Up()));
            transformComponent.Position = spawnPos;
            ECB.SetComponent(sortKey, entity, transformComponent);
            ECB.AddComponent(sortKey, entity, new TargetComponent() { Value = closestHitCollector.ClosestHit.Entity });
        }
    }
}