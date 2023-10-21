using System;
using Unity.Collections;
using Unity.Entities;
using UnityMonoBehaviour.SpawningConfig;

namespace Components.Spawning
{
    public struct WaveInfoComponent : IComponentData, IEnableableComponent
    {
        public int MaxSpawnedAtOnce;
        public float TimeBetweenSpawns;
        public int AlreadySpawnedCount;
    }
}