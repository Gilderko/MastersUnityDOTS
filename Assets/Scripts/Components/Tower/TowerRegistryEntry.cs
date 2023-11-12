using Unity.Entities;

namespace Components
{
    public struct TowerRegistryEntry : IBufferElementData
    {
        public int BuildPrice;
        public float BuildRadius;
        
        public int Level;
        public bool Buildable;
        public TowerType Type;
        
        public BlobAssetReference<TowerConfigComponent> Config;
        
        public Entity TowerPrefab;
        public Entity DummyPrefab;
    }
}