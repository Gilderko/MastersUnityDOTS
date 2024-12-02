using Unity.Entities;

namespace Components.Enemy
{
    public struct EnemyConfigComponent : IComponentData
    {
        public BlobAssetReference<EnemyConfig> Config;
    }

    public struct EnemyConfig
    {
        public int Reward;
        public float Speed;
        public int AttackDamage;
    }
}