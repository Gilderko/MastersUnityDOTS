using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class DummyTowerAuthoring : MonoBehaviour
    {
        public GameObject OriginalTowerPrefab;
        public GameObject VisualChild;
        public int TowerPrice;
        public float BuildRadius;
        
        private class DummyTowerBaker : Baker<DummyTowerAuthoring>
        {
            public override void Bake(DummyTowerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var visual = GetEntity(authoring.VisualChild, TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new TowerDummyComponent()
                {
                    TowerPrefab = GetEntity(authoring.OriginalTowerPrefab, TransformUsageFlags.Dynamic),
                    Visual = visual,
                    BuildPrice = authoring.TowerPrice,
                    BuildRadius = authoring.BuildRadius
                });
            }
        }
    }
}