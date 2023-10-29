using Unity.Entities;
using Unity.Physics;

namespace Components
{
    public struct TowerPlacementEntry : IBufferElementData
    {
        public RaycastInput Value;
        public int TowerIndex;
    }
}