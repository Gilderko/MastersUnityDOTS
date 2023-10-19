using Unity.Entities;

namespace Components
{
    public struct LimitedLifeComponent : IComponentData
    {
        public float LifeRemainingSeconds;
    }
}