using Unity.Entities;
using Unity.Physics;

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
        public ProjectileType ProjectileType;
        public float ExplosiveDamageRadius;
        public CollisionFilter CollisionFilter;
    }
}