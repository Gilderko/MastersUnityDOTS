using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityMonoBehaviour;

namespace Components
{
    public class HealthBarManagedComponent : IComponentData, IDisposable
    {
        public HealthSlider HealthSlider;
        public float3 Offset;
        
        public void Dispose()
        {
            if (HealthSlider == null)
            {
                return;
            }
            
            HealthSlider.gameObject.SetActive(false);
        }
    }
}