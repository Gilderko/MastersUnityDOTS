using Unity.Entities;

namespace Components.Shared
{
    public struct EntityReferenceBufferElement : IBufferElementData
    {
        public Entity EntityRef;
    }
}