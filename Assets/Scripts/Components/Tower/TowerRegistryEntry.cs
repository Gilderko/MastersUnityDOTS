using Unity.Entities;

namespace Components
{
    public struct TowerRegistryEntry : IBufferElementData
    {
        public int TowerLevel;
        public TowerType TowerType;
        
        public int BuildPrice;
        public float BuildRadius;
        public bool Buildable;
        
        public Entity TowerPrefab;
        public Entity DummyPrefab;
    }
}