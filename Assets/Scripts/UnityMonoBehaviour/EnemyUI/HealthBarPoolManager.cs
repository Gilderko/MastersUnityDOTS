using System;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace UnityMonoBehaviour
{
    public class HealthBarPoolManager : MonoBehaviour
    {
        [SerializeField] private HealthSlider SliderPrefab;
        
        public HealthSlider GetNewSlider()
        {
            return Instantiate(SliderPrefab, transform);
        }
    }
}