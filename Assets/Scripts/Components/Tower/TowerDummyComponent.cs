using Unity.Entities;

namespace Components
{
    public struct TowerDummyComponent : IComponentData
    {
        public int BuildPrice;
        public float BuildRadius;
        public Entity TowerPrefab;
        public Entity Visual;
    }
}