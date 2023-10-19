using Unity.Entities;

namespace Components
{
    public struct TargetComponent : IComponentData
    {
        public Entity Value;
    }
}