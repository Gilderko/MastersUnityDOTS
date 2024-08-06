using Components;
using Components.Enemy;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float Speed;
        public int Health;
        public int EnemyReward;
        
        public Vector3 HealthBarOffset = Vector3.up * 2.5f;
    
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
                    Value = authoring.Health,
                    InitialValue = authoring.Health
                });

                AddBuffer<HitDataComponent>(entity);
                AddComponentObject<HealthBarManagedComponent>(entity, new HealthBarManagedComponent()
                {
                    HealthSlider = null,
                    Offset = authoring.HealthBarOffset
                });
            }
        }
    }
}


