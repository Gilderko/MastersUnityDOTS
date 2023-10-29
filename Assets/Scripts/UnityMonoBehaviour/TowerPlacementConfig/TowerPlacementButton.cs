using Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace UnityMonoBehaviour.TowerPlacementConfig
{
    public class TowerPlacementButton : MonoBehaviour
    {
        private World _world;

        private Entity _prefabEntity;
        private Entity _prefabDummyEntity;

        public void Initialize(Entity prefabDummyEntity, Entity prefabEntity)
        {
            _prefabDummyEntity = prefabDummyEntity;
            _prefabEntity = prefabEntity;
            _world = World.DefaultGameObjectInjectionWorld;
        }
        
        public void ButtonClicked()
        {
            if (_world.EntityManager.CreateEntityQuery(typeof(TowerDummyComponent)).TryGetSingletonEntity<TowerDummyComponent>(out var currentTower ))
            {
                _world.EntityManager.DestroyEntity(currentTower);
            }
            
            var newTower = _world.EntityManager.Instantiate(_prefabDummyEntity);
            _world.EntityManager.AddComponentData(newTower, new TowerDummyComponent()
            {
                TowerPrefab = _prefabEntity
            });
        }
    }
}