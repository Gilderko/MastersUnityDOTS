using System;
using Unity.Collections;
using Unity.Entities;
using UnityMonoBehaviour.SpawningConfig;

namespace Components.Spawning
{
    public struct WaveInfoComponent : IComponentData, IEnableableComponent
    {
        public float TimeBetweenSpawns;
    }
}