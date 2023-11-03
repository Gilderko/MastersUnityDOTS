using Unity.Entities;

namespace Components
{
    public struct TowerDummyComponent : IComponentData
    {
        public int BuildPrice;
        public Entity TowerPrefab;
        public Entity Visual;
    }
}