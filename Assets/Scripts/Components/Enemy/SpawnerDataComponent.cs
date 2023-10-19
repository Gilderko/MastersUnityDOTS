using Unity.Entities;

public struct SpawnerDataComponent : IComponentData
{
    public Entity Prefab;
    public float Timer;
    public float TimeToNextSpawn;
}
