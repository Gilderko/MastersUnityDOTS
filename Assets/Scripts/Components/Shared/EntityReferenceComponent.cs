using Unity.Entities;

namespace Components.Shared
{
    public struct EntityReferenceComponent : IComponentData
    {
        public Entity EntityRef;
    }
}