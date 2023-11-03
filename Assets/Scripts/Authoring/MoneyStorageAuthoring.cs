using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class MoneyStorageAuthoring : MonoBehaviour
    {
        public int StartingPriceAmmount = 100;
        
        private class MoneyStorageBaker : Baker<MoneyStorageAuthoring>
        {
            public override void Bake(MoneyStorageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new MoneyComponent()
                {
                    CurrentMoney = authoring.StartingPriceAmmount
                });

                AddBuffer<AddMoneyElement>(entity);
            }
        }
    }
}