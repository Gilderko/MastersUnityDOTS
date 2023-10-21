using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct RandomComponent : IComponentData
    {
        public Random RandomValue;
    }
}