using Components.Enemy;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct PathFollowerAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<SpeedComponent> _speed;
        private readonly RefRW<NextPathIndexComponent> _pathIndex;
        private readonly RefRO<PathComponent> _pathAsset;

        public LocalTransform Position
        {
            get => _transform.ValueRO;
            set => _transform.ValueRW = value;
        }
        
        public void FollowPath(float time)
        {
            ref var path = ref _pathAsset.ValueRO.Path.Value.Waypoints;
            var direction = path[_pathIndex.ValueRO.NextIndex] - Position.Position;
            
            if (math.distance(Position.Position, path[_pathIndex.ValueRO.NextIndex]) < 0.1f)
            {
                _pathIndex.ValueRW.NextIndex = (_pathIndex.ValueRO.NextIndex + 1) % path.Length;
            }
            var movementSpeed = _speed.IsValid ? _speed.ValueRO.Value : 1;
            
            _transform.ValueRW.Position += math.normalize(direction) * time * movementSpeed;
            _transform.ValueRW.Rotation = quaternion.LookRotationSafe(direction, math.up()); 
        }

        public bool HasReachedEndOfPath()
        {
            ref var path = ref _pathAsset.ValueRO.Path.Value.Waypoints;
            return math.distance(Position.Position, path[path.Length - 1]) < 0.1f;
        }
    }
}