using Components;
using Components.Enemy;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float Speed;
        public float Health;
    
        public class EnemyBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
        
                AddComponent(entity, new SpeedComponent()
                {
                    Value = authoring.Speed
                });
                AddComponent(entity, new HealthComponent()
                {
                    Value = authoring.Health 
                });
            }
        }
    }
}


