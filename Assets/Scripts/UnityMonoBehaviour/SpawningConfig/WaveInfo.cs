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
        [SerializeField] private EntityAndProbability[] _enemyTypesToSpawn = Array.Empty<EntityAndProbability>();

        public int MaxSpawnedAtOnce => _maxSpawnedAtOnce;
        public float TimeBetweenSpawns => _timeBetweenSpawns;
        public EntityAndProbability[] EntitiesToSpawn => _enemyTypesToSpawn;
    }
}