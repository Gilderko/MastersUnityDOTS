using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class SpawnerAuthoringAuthoring : MonoBehaviour
    {
        public float Timer;
        public GameObject EnemyPrefab;
        public List<Transform> Path => GetComponentsInChildren<Transform>().Where(go => go.gameObject != this.gameObject).ToList();
        
        private class SpawnerAuthoringBaker : Baker<SpawnerAuthoringAuthoring>
        {
            public override void Bake(SpawnerAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                BlobAssetReference<BlobPath> blobPathReference;
                using (var bb = new BlobBuilder(Allocator.Temp))
                {
                    ref var blobPath = ref bb.ConstructRoot<BlobPath>();

                    var waypoints = bb.Allocate(ref blobPath.Waypoints, authoring.Path.Count);
                    for (var i = 0; i < authoring.Path.Count; i++)
                    {
                        waypoints[i] = authoring.Path.ElementAt(i).position;
                    }
                    blobPathReference = bb.CreateBlobAssetReference<BlobPath>(Allocator.Persistent);
                }
                
                AddBlobAsset(ref blobPathReference, out var _);
                AddComponent(entity, new PathComponent() { Path = blobPathReference });
                AddComponent(entity, new SpawnerDataComponent()
                {
                    Prefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                    Timer = authoring.Timer,
                    TimeToNextSpawn = authoring.Timer,
                });
            }
        }
    }
}