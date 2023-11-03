using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct ProjectileAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<TargetComponent> _targetComponent;
        private readonly RefRO<LimitedLifeComponent> _lifeComponent;

        private readonly RefRO<ProjectileConfigComponent> _projectileConfig;

        public float Speed => _projectileConfig.ValueRO.Config.Value.Speed;
        
        public TargetComponent Target => _targetComponent.ValueRO;
    }
}