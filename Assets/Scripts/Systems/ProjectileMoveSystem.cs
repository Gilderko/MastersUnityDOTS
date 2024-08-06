using Systems.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct ProjectileMoveSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _positionLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbBos = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var deltaTime = SystemAPI.Time.DeltaTime;
            _positionLookup.Update(ref state);

            var moveJob = new ProjectileMoveJob()
            {
                DeltaTime = deltaTime,
                PositionLookup = _positionLookup
            };

            moveJob.ScheduleParallel();
        }
    }
}
