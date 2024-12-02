using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityMonoBehaviour;

namespace Components
{
    public class HealthBarManagedComponent : IComponentData, IDisposable
    {
        public HealthSlider HealthSlider;
        public float3 Offset = new float3(0,0,0);
        public float3 ScaleOverride = new float3(1,1,1);
        
        public void Dispose()
        {
            if (HealthSlider == null)
            {
                return;
            }
            
            GameObject.Destroy(HealthSlider.gameObject);
        }
    }
}