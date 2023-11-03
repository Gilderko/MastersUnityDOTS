using Components;
using Components.Enemy;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float Speed;
        public int Health;
        public int EnemyReward;
    
        public class EnemyBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                BlobAssetReference<EnemyConfig> blobAssetReference;
                using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var tc = ref bb.ConstructRoot<EnemyConfig>();
                    tc.Reward = authoring.EnemyReward;
                    tc.Speed = authoring.Speed;
                   
                    blobAssetReference = bb.CreateBlobAssetReference<EnemyConfig>(Unity.Collections.Allocator.Persistent);
                }
            
                AddBlobAsset(ref blobAssetReference, out var _);
                AddComponent(entity, new EnemyConfigComponent()
                {
                    Config = blobAssetReference
                });
                
                AddComponent(entity, new HealthComponent()
                {
                    Value = authoring.Health 
                });

                AddBuffer<HitDataComponent>(entity);
            }
        }
    }
}


