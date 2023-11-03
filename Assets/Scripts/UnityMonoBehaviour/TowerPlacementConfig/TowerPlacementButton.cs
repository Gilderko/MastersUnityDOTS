using Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace UnityMonoBehaviour.TowerPlacementConfig
{
    public class TowerPlacementButton : MonoBehaviour
    {
        private World _world;

        private Entity _prefabDummyEntity;

        public void Initialize(Entity prefabDummyEntity)
        {
            _prefabDummyEntity = prefabDummyEntity;
            _world = World.DefaultGameObjectInjectionWorld;
        }
        
        public void ButtonClicked()
        {
            if (_world.EntityManager.CreateEntityQuery(typeof(TowerDummyComponent)).TryGetSingletonEntity<TowerDummyComponent>(out var currentTower ))
            {
                _world.EntityManager.DestroyEntity(currentTower);
            }
            
            _world.EntityManager.Instantiate(_prefabDummyEntity);
        }
    }
}