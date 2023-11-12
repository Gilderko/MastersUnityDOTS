using Components;
using Unity.Entities;
using UnityEngine;

namespace UnityMonoBehaviour.TowerUI
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
            
            foreach (var spawnEntry in _spawnableTowers)
            {
                if (!spawnEntry.Buildable)
                {
                    continue;
                }
                
                var spawnButtons = Instantiate(_towerPlacementPrefab, transform);
                spawnButtons.Initialize(spawnEntry);
            }
        }
    }
}