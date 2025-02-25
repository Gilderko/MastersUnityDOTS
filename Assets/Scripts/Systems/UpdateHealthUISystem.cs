﻿using Components;
using Components.Enemy;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityMonoBehaviour;

namespace Systems
{
    /// <summary>
    /// Updates the position and health display values of all entities which have both the HealthComponent and HealthBarManagedComponent
    /// </summary>
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
                healthSlider.DisplayHealth(Mathf.Max(health.Value, 0) / (float) health.InitialValue);

                healthUI.HealthSlider = healthSlider;
            }
        }
    }
}