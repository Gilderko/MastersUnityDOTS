using System;
using UnityEngine;

namespace UnityMonoBehaviour.SpawningConfig
{
    [Serializable]
    public class WaveInfo
    {
        [Header("WaveInformation")]
    
        [SerializeField] private float _timeBetweenSpawns;
        [SerializeField] private EntitySpawnMonoData[] _enemyTypesToSpawn = Array.Empty<EntitySpawnMonoData>();

        public float TimeBetweenSpawns => _timeBetweenSpawns;
        public EntitySpawnMonoData[] EntitiesToSpawn => _enemyTypesToSpawn;
    }
}