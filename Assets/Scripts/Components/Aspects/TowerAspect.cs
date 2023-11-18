using Unity.Entities;
using Unity.Transforms;

namespace Components.Aspects
{
    public readonly partial struct TowerAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<TowerConfigAsset> _towerConfig;
        private readonly RefRW<TimerComponent> _timer;
        private readonly RefRO<TowerDataComponent> _towerDataComponent;
        private readonly RefRO<TowerHeadComponent> _towerHeadComponent;

        public TowerHeadComponent TowerHead => _towerHeadComponent.ValueRO;
        
        public TowerDataComponent TowerData => _towerDataComponent.ValueRO;
        
        public TowerConfigAsset TowerConfig => _towerConfig.ValueRO;

        public TimerComponent ProjectileTimerComponent => _timer.ValueRO;

        public void DecrementProjectileTimer(float deltaTime)
        {
            _timer.ValueRW.TimerValue -= deltaTime;
        }

        public void ResetProjectileTimer()
        {
            _timer.ValueRW.TimerValue = TowerConfig.Config.Value.FireRate;
        }
    }
}