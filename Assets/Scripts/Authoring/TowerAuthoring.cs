using Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace Authoring
{
    public class TowerAuthoring : MonoBehaviour
    {
        public GameObject TowerHead;
        
        public ProjectileAuthoring Projectile;
        public float FireRate;
        public float Range;
        public int Level = 0;
        public TowerType TowerType = TowerType.Normal;

        public class Baker : Baker<TowerAuthoring>
        {
            public override void Bake(TowerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
            
                AddComponent(entity, new TowerDataComponent()
                {
                    ProjectilePrefab = GetEntity(authoring.Projectile.gameObject, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity,new TimerComponent()
                {
                    TimerValue = 1,
                });

                var bar = authoring.GenerateTowerBlobAsset();

                AddBlobAsset(ref bar, out var _);
                AddComponent(entity, new TowerConfigAsset()
                {
                    Config = bar
                });
                
                AddComponent(entity, new TowerHeadComponent()
                {
                    TowerHead = GetEntity(authoring.TowerHead, TransformUsageFlags.Dynamic)
                });
            }
        }
        
        /// <summary>
        /// Generate shared blob config for the appropriate new tower
        /// </summary>
        /// <returns></returns>
        public BlobAssetReference<TowerConfig> GenerateTowerBlobAsset()
        {
            var filter = CollisionFilter.Zero;
            filter.CollidesWith = Projectile.GetComponent<PhysicsShapeAuthoring>().CollidesWith.Value;
            filter.BelongsTo = Projectile.GetComponent<PhysicsShapeAuthoring>().BelongsTo.Value;
            
            BlobAssetReference<TowerConfig> bar;
            using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
            {
                ref var tc = ref bb.ConstructRoot<TowerConfig>();
                tc.FireRate = FireRate;
                tc.FireRange = Range;
                tc.Filter = filter;

                tc.ProjectileDamage = Projectile.Damage;
                tc.TowerType = TowerType;
                tc.Level = Level;

                bar = bb.CreateBlobAssetReference<TowerConfig>(Unity.Collections.Allocator.Persistent);
            }

            return bar;
        }
    }
}
