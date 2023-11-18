using Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace Authoring
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public float Speed;
        public GameObject ImpactVFX;
        public float MaxLifeSeconds;
        public int Damage;
        public ProjectileType ProjectileType;
        public float ExplosiveDamageRadius;

        public class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ImpactComponent() { VFXImpactPrefab = GetEntity(authoring.ImpactVFX, TransformUsageFlags.Dynamic) });
                AddComponent(entity, new LimitedLifeComponent() { LifeRemainingSeconds = authoring.MaxLifeSeconds });
                
                var filter = CollisionFilter.Zero;
                filter.CollidesWith = authoring.GetComponent<PhysicsShapeAuthoring>().CollidesWith.Value;
                filter.BelongsTo = authoring.GetComponent<PhysicsShapeAuthoring>().BelongsTo.Value;
                
                BlobAssetReference<ProjectileConfig> blobAssetReference;
                using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var tc = ref bb.ConstructRoot<ProjectileConfig>();
                    tc.Damage = authoring.Damage;
                    tc.Speed = authoring.Speed;
                    tc.ProjectileType = authoring.ProjectileType;
                    tc.ExplosiveDamageRadius = authoring.ExplosiveDamageRadius;
                    tc.CollisionFilter = filter;
                    
                    blobAssetReference = bb.CreateBlobAssetReference<ProjectileConfig>(Unity.Collections.Allocator.Persistent);
                }
            
                AddBlobAsset(ref blobAssetReference, out var _);
                AddComponent(entity, new ProjectileConfigComponent()
                {
                    Config = blobAssetReference
                });
                
            }
        }
    }
}