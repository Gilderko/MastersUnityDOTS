using System;
using UnityEngine;

namespace UnityMonoBehaviour.SpawningConfig
{
    [Serializable]
    public class WaveInfo
    {
        [Header("WaveInformation")]
    
        [SerializeField] private int _maxSpawnedAtOnce;
        [SerializeField] private float _timeBetweenSpawns;
        [SerializeField] private EntitySpawnMonoData[] _enemyTypesToSpawn = Array.Empty<EntitySpawnMonoData>();

        public int MaxSpawnedAtOnce => _maxSpawnedAtOnce;
        public float TimeBetweenSpawns => _timeBetweenSpawns;
        public EntitySpawnMonoData[] EntitiesToSpawn => _enemyTypesToSpawn;
    }
}