using Unity.Entities;
using UnityEngine;

namespace Components.Spawning
{
    public struct EntitySpawnData : IBufferElementData
    {
        public Entity EntityToSpawn;
        public int CountToSpawn;
        public Entity VFXToSpawn;
        public SpawnTransformModifierData ParticleSystemSpawnModifier;
    }
}