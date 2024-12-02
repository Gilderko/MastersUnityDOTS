using Components;
using Components.Enemy;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FriendlyBaseAuthoringAuthoring : MonoBehaviour
    {
        public int HealthValue = 150;
        public Vector3 HealthBarOffset = Vector3.up * 2.5f;
        public Vector3 ScaleOverride = new Vector3(1.5f, 1.1f, 0f);
        
        private class FriendlyBaseAuthoringBaker : Baker<FriendlyBaseAuthoringAuthoring>
        {
            
            public override void Bake(FriendlyBaseAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new FriendlyBaseTag());
                AddComponent(entity, new HealthComponent()
                {
                    InitialValue = 200,
                    Value = 200
                });
                
                AddComponentObject<HealthBarManagedComponent>(entity, new HealthBarManagedComponent()
                {
                    HealthSlider = null,
                    ScaleOverride = authoring.ScaleOverride,
                    Offset = authoring.HealthBarOffset
                });
            }
        }
    }
}