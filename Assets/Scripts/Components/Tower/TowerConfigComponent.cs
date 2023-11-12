using Unity.Entities;
using Unity.Physics;

namespace Components
{
    public struct TowerDataComponent : IComponentData
    {
        public Entity ProjectilePrefab;  
    }
    
    public struct TowerConfigComponent
    {
        public float FireRate;                 
        public CollisionFilter Filter;      
        public float FireRange;
        public float ProjectileDamage;
    }

    public struct TowerConfigAsset : IComponentData
    {
        public BlobAssetReference<TowerConfigComponent> Config;    
    }
}