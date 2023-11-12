using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class DummyTowerAuthoring : MonoBehaviour
    {
        public GameObject VisualChild;
        
        private class DummyTowerBaker : Baker<DummyTowerAuthoring>
        {
            public override void Bake(DummyTowerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var visual = GetEntity(authoring.VisualChild, TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new TowerDummyComponent()
                {
                    Visual = visual,
                });
            }
        }
    }
}