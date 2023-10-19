using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct ProjectileAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<SpeedComponent> _speedComponent;
        private readonly RefRO<TargetComponent> _targetComponent;
        private readonly RefRO<LimitedLifeComponent> _lifeComponent;

        public SpeedComponent Speed => _speedComponent.ValueRO;
        
        public TargetComponent Target => _targetComponent.ValueRO;
    }
}