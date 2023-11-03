using Unity.Entities;

namespace Components
{
    public struct ProjectileConfigComponent : IComponentData
    {
        public BlobAssetReference<ProjectileConfig> Config;
    }

    public struct ProjectileConfig
    {
        public int Damage;
        public float Speed;
    }
}