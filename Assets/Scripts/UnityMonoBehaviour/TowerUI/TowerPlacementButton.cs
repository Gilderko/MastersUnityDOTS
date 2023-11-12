using Components;
using Unity.Entities;
using UnityEngine;

namespace UnityMonoBehaviour.TowerUI
{
    public class TowerPlacementButton : MonoBehaviour
    {
        private World _world;

        private TowerRegistryEntry _towerPrefabEntry;

        public void Initialize(TowerRegistryEntry towerEntry)
        {
            _towerPrefabEntry = towerEntry;
            _world = World.DefaultGameObjectInjectionWorld;
        }
        
        public void ButtonClicked()
        {
            if (_world.EntityManager.CreateEntityQuery(typeof(TowerDummyComponent)).TryGetSingletonEntity<TowerDummyComponent>(out var currentTower ))
            {
                _world.EntityManager.DestroyEntity(currentTower);
            }
            
            var newDummyTowerEntity = _world.EntityManager.Instantiate(_towerPrefabEntry.DummyPrefab);
            var currentComp = _world.EntityManager.GetComponentData<TowerDummyComponent>(newDummyTowerEntity);
            currentComp.BuildRadius = _towerPrefabEntry.BuildRadius;
            currentComp.BuildPrice = _towerPrefabEntry.BuildPrice;
            currentComp.TowerPrefab = _towerPrefabEntry.TowerPrefab;
            _world.EntityManager.SetComponentData<TowerDummyComponent>(newDummyTowerEntity, currentComp);
        }
    }
}