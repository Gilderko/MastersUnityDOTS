using Unity.Entities;
using Unity.Mathematics;

namespace Components.Enemy
{
    public struct BlobPath
    {
        public BlobArray<float3> Waypoints;
    }

    public struct PathComponent : IComponentData
    {
        public BlobAssetReference<BlobPath> Path;
    }
}