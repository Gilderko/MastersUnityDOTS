using Unity.Entities;

namespace Components.Enemy
{
    public struct HitDataComponent : IBufferElementData
    {
        public int DamageToTake;
    }
}