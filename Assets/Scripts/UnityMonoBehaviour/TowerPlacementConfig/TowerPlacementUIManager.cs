using Components;
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

namespace UnityMonoBehaviour.TowerPlacementConfig
{
    public class TowerPlacementUIManager : MonoBehaviour
    {
        [SerializeField]
        private TowerPlacementButton _towerPlacementPrefab;

        private DynamicBuffer<TowerRegistryEntry> _spawnableTowers;
        private World _world;
        private Entity _currentTower;

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _spawnableTowers = _world.EntityManager.CreateEntityQuery(typeof(TowerRegistryEntry)
                ).GetSingletonBuffer<TowerRegistryEntry>();
            
            for (var i = 0; i < _spawnableTowers.Length; i++)
            {
                var spawnButtons = Instantiate(_towerPlacementPrefab, transform);
                spawnButtons.Initialize(_spawnableTowers[i].DummyPrefab, _spawnableTowers[i].Prefab);
            }
        }
    }
}