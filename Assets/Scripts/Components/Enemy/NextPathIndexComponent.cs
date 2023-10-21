using Unity.Entities;

namespace Components.Enemy
{
    public struct NextPathIndexComponent : IComponentData
    {
        public int NextIndex;
    }
}