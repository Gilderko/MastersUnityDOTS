using Components;
using Components.Enemy;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityMonoBehaviour;

namespace Systems
{
    [UpdateAfter(typeof(ProcessProjectileHitsSystem))]
    public partial class UpdateHealthUISystem : SystemBase
    {
        private HealthBarPoolManager _healthBarPoolManager;

        protected override void OnStartRunning()
        {
            _healthBarPoolManager = Object.FindObjectOfType<HealthBarPoolManager>();
        }
        
        protected override void OnUpdate()
        {
            foreach (var (entitiesTrans, health, healthUI, entity) in SystemAPI.Query<LocalTransform, HealthComponent, HealthBarManagedComponent>().WithEntityAccess())
            {
                var healthSlider = healthUI.HealthSlider;
                if (healthSlider == null)
                {
                    healthSlider = _healthBarPoolManager.GetNewSlider();
                }

                healthSlider.transform.position = entitiesTrans.Position + healthUI.Offset;
                healthSlider.transform.localScale = healthUI.ScaleOverride;
                healthSlider.DisplayHealth(health.Value / (float) health.InitialValue);

                healthUI.HealthSlider = healthSlider;
            }
        }
    }
}