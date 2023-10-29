using Unity.Entities;
using Unity.Physics.Authoring;

namespace Components
{
    public struct TowerPlacementLayersComponent : IComponentData
    {
        public PhysicsCategoryTags BelongsToPlacement;
        public PhysicsCategoryTags CollidesWithPlacement;
        
        public PhysicsCategoryTags BelongsToMove;
        public PhysicsCategoryTags CollidesWithMove;
    }
}