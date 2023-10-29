using Unity.Entities;

namespace Components
{
    public struct TowerDummyComponent : IComponentData
    {
        public Entity TowerPrefab;
    }
}