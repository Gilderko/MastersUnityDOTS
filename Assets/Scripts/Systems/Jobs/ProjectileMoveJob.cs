using Components.Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace Systems.Jobs
{
    /// <summary>
    /// Move all Projectiles towards their enemy targets or forwards (if they dont have a target) in parallel
    /// </summary>
    [BurstCompile]
    public partial struct ProjectileMoveJob : IJobEntity
    {
        // We can write in parallel since each Execution method only move a single projectile
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> PositionLookup;
        public float DeltaTime;
        
        [BurstCompile]
        public void Execute(ProjectileAspect projectileAspect)
        {
            if (projectileAspect.Target.Value == Entity.Null)
            {
                return;
            }

            var projectilePosition = PositionLookup[projectileAspect.Entity];
            var targetDir = projectilePosition.Forward();
            if (PositionLookup.TryGetComponent(projectileAspect.Target.Value, out var targetPosition))
            {
                targetDir = targetPosition.Position - projectilePosition.Position;
            }
            
            var newRotation = quaternion.LookRotationSafe(targetDir, math.up()); 
            var newPosition = projectilePosition.Position + projectileAspect.Speed * DeltaTime * projectilePosition.Forward();

            projectilePosition.Position = newPosition;
            projectilePosition.Rotation = newRotation;

            PositionLookup[projectileAspect.Entity] = projectilePosition;
        }
    }
}