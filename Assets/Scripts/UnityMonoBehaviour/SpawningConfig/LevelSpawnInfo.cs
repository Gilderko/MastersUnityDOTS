using System;
using UnityEngine;

namespace UnityMonoBehaviour.SpawningConfig
{
    [Serializable]
    public class LevelSpawnInfo
    {
        [Header("Level wave")]
        [SerializeField] private WaveInfo[] _waves;

        public WaveInfo[] Waves => _waves;
    }
}