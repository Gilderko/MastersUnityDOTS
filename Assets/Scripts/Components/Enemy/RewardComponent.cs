using Unity.Entities;

namespace Components.Enemy
{
    public struct RewardComponent : ISharedComponentData
    {
        public int MoneyReward;
    }
}