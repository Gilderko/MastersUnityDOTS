using Components.Aspects;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Jobs
{
    /// <summary>
    /// Moves all entities along the path in parallel
    /// </summary>
    [BurstCompile]
    public partial struct MoveEnemyJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        public void Execute(PathFollowerAspect pathFollower, [EntityIndexInQuery] int sortKey)
        {
            pathFollower.FollowPath(DeltaTime);
            if(pathFollower.HasReachedEndOfPath())
            {
                ECB.DestroyEntity(sortKey, pathFollower.Entity);
            }
        }
    }
}