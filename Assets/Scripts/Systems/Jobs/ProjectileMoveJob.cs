using Components.Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace Systems.Jobs
{
    [BurstCompile]
    public partial struct ProjectileMoveJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> PositionLookup;
        public float DeltaTime;
        
        [BurstCompile]
        public void Execute(ProjectileAspect projectileAspect)
        {
            var projectilePosition = PositionLookup[projectileAspect.Entity];
            var targetPosition = PositionLookup[projectileAspect.Target.Value];
            
            var targetDir = targetPosition.Position - projectilePosition.Position;
            
            var newRotation = quaternion.LookRotationSafe(targetDir, math.up()); 
            var newPosition = projectilePosition.Position + projectileAspect.Speed.Value * DeltaTime * projectilePosition.Forward();

            projectilePosition.Position = newPosition;
            projectilePosition.Rotation = newRotation;

            PositionLookup[projectileAspect.Entity] = projectilePosition;
        }
    }
}