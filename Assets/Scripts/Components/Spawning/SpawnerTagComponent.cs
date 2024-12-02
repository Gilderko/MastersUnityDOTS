using Unity.Entities;

namespace Components.Spawning
{
    public struct SpawnerTagComponent : IComponentData
    {
        public int TotalEnemiesKilled;
        public int TotalEnemies;
    }
}