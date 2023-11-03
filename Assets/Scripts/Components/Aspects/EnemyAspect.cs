using Components.Enemy;
using Unity.Entities;

namespace Components.Aspects
{
    public readonly partial struct EnemyAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<HealthComponent> _healthComponent;
        private readonly DynamicBuffer<HitDataComponent> _hitDataComponents;
        private readonly RefRO<EnemyConfigComponent> _configComponent;
        
        public int CurrentHealth => _healthComponent.ValueRO.Value;

        public EnemyConfig EnemyConfig => _configComponent.ValueRO.Config.Value;
        
        public void EvaluateHealthBuffer()
        {
            foreach (var hitElement in _hitDataComponents)
            {
                _healthComponent.ValueRW.Value -= hitElement.DamageToTake;
            }
            
            _hitDataComponents.Clear();
        }
    }
}