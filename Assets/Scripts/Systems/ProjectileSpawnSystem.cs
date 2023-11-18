using Systems.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial struct ProjectileSpawnSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _positionLookup;
        private ComponentLookup<LocalToWorld> _worldPosLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
            _worldPosLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbBos = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            _positionLookup.Update(ref state);
            _worldPosLookup.Update(ref state);
            
            var spawnProjectileJob = new ProjectileSpawnJob()
            {
                ECB = ecbBos.AsParallelWriter(),
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                TransformLookup = _positionLookup,
                WorldLookup = _worldPosLookup
            };

            spawnProjectileJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
