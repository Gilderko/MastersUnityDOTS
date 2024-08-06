using Unity.Entities;
using Unity.Physics;

namespace Components
{
    public struct TowerDataComponent : IComponentData
    {
        public Entity ProjectilePrefab;  
    }
    
    public struct TowerConfig
    {
        public float FireRate;                 
        public CollisionFilter Filter;      
        public float FireRange;
        public float ProjectileDamage;
        public int Level;
        public TowerType TowerType;
    }

    public struct TowerConfigAsset : IComponentData
    {
        public BlobAssetReference<TowerConfig> Config;    
    }
}