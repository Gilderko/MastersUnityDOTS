using Unity.Entities;

namespace Components.Enemy
{
    public struct HealthComponent : IComponentData
    {
        public int Value;
        public int InitialValue;
    }
}