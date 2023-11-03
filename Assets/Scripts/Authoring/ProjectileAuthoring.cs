using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public float Speed;
        public GameObject ImpactVFX;
        public float MaxLifeSeconds;
        public int Damage;

        public class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ImpactComponent() { VFXImpactPrefab = GetEntity(authoring.ImpactVFX, TransformUsageFlags.Dynamic) });
                AddComponent(entity, new LimitedLifeComponent() { LifeRemainingSeconds = authoring.MaxLifeSeconds });
                
                BlobAssetReference<ProjectileConfig> blobAssetReference;
                using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var tc = ref bb.ConstructRoot<ProjectileConfig>();
                    tc.Damage = authoring.Damage;
                    tc.Speed = authoring.Speed;
                   
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