using System;
using UnityEngine;

namespace UnityMonoBehaviour.SpawningConfig
{
    [Serializable]
    public class EntitySpawnMonoData
    {
        [SerializeField] private GameObject _entityToSpawn;
        [SerializeField] private int _countToSpawn;
        
        [SerializeField] private GameObject _vfxToSpawn;
        [SerializeField] private SpawnTransformModifier _particleSystemSpawnModifier;
        
        [SerializeField] private AudioClip _sfxToSpawn;
        
        public int CountEntitiesToSpawn => _countToSpawn;
        public GameObject SpawnableEntity => _entityToSpawn;
        public GameObject VFXToInstantWhenSpawned => _vfxToSpawn;
        public SpawnTransformModifier ParticleSpawnTransformModifier => _particleSystemSpawnModifier;
        public AudioClip ClipWhenSpawned => _sfxToSpawn;
    }
}