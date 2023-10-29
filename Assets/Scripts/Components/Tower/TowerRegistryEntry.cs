using Unity.Entities;

namespace Components
{
    public struct TowerRegistryEntry : IBufferElementData
    {
        public Entity Prefab;
        public Entity DummyPrefab;
    }
}