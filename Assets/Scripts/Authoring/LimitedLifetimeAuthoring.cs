using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class LimitedLifetimeAuthoringAuthoring : MonoBehaviour
    {
        public float MaxLifetimeSeconds;
        
        public class LimitedLifetimeAuthoringBaker : Baker<LimitedLifetimeAuthoringAuthoring>
        {
            public override void Bake(LimitedLifetimeAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new LimitedLifeComponent() {LifeRemainingSeconds = authoring.MaxLifetimeSeconds});
            }
        }
    }
}