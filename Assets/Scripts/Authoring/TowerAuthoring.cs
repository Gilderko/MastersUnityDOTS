using Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace Authoring
{
    public class TowerAuthoring : MonoBehaviour
    {
        public ProjectileAuthoring Projectile;
        public float FireRate;
        public float Range;

        public class Baker : Baker<TowerAuthoring>
        {
            public override void Bake(TowerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var filter = CollisionFilter.Default;
                filter.CollidesWith = authoring.Projectile.GetComponent<PhysicsShapeAuthoring>().CollidesWith.Value;
                filter.BelongsTo = authoring.Projectile.GetComponent<PhysicsShapeAuthoring>().BelongsTo.Value;
            
                AddComponent(entity, new TowerDataComponent()
                {
                    ProjectilePrefab = GetEntity(authoring.Projectile.gameObject, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity,new TimerComponent()
                {
                    TimerValue = 0,
                });

                BlobAssetReference<TowerConfigComponent> bar;
                using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var tc = ref bb.ConstructRoot<TowerConfigComponent>();
                    tc.FireRate = authoring.FireRate;
                    tc.FireRange = authoring.Range;
                    tc.Filter = filter;
                    tc.ProjectileDamage = authoring.Projectile.Damage;
                   
                    bar = bb.CreateBlobAssetReference<TowerConfigComponent>(Unity.Collections.Allocator.Persistent);
                }
            
                AddBlobAsset(ref bar, out var _);
                AddComponent(entity, new TowerConfigAsset()
                {
                    Config = bar
                });
            } 
        }
    }
}
