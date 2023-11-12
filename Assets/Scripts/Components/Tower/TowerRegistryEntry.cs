using Unity.Entities;

namespace Components
{
    public struct TowerRegistryEntry : IBufferElementData
    {
        public int BuildPrice;
        public float BuildRadius;
        public bool Buildable;
        
        public BlobAssetReference<TowerConfigComponent> Config;
        
        public Entity TowerPrefab;
        public Entity DummyPrefab;
    }
}