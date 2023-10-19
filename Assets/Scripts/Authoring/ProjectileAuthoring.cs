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

        public class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SpeedComponent() { Value = authoring.Speed });
                AddComponent(entity, new ImpactComponent() { VFXImpactPrefab = GetEntity(authoring.ImpactVFX, TransformUsageFlags.Dynamic) });
                AddComponent(entity, new LimitedLifeComponent() { LifeRemainingSeconds = authoring.MaxLifeSeconds });
            }
        }
    }
}