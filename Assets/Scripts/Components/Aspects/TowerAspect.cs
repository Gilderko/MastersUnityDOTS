using Unity.Entities;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct TowerAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<TowerConfigAsset> _towerConfig;
        private readonly RefRW<TimerComponent> _timer;
        private readonly RefRO<TowerDataComponent> _towerDataComponent;

        public TowerDataComponent TowerData => _towerDataComponent.ValueRO;
        
        public TowerConfigAsset TowerConfig => _towerConfig.ValueRO;

        public LocalTransform Transform => _transform.ValueRO;

        public TimerComponent ProjectileTimerComponent => _timer.ValueRO;

        public void DecrementProjectileTimer(float deltaTime)
        {
            _timer.ValueRW.TimerValue -= deltaTime;
        }

        public void ResetProjectileTimer()
        {
            _timer.ValueRW.TimerValue = TowerConfig.Config.Value.Timer;
        }
    }
}