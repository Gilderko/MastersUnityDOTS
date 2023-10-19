using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct BlobPath
{
    public BlobArray<float3> Waypoints;
}

public struct PathComponent : IComponentData
{
    public BlobAssetReference<BlobPath> Path;
}