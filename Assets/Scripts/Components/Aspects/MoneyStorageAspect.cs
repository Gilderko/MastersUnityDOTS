using Unity.Entities;

namespace Components.Aspects
{
    public readonly partial struct MoneyStorageAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<MoneyComponent> _moneyStorage;
        private readonly DynamicBuffer<AddMoneyElement> _addMoneyElements;

        public int CurrentMoney => _moneyStorage.ValueRO.CurrentMoney;
        
        public void EvaluateMoneyBuffer()
        {
            foreach (var moneyElement in _addMoneyElements)
            {
                _moneyStorage.ValueRW.CurrentMoney += moneyElement.MoneyReward;
            }
            
            _addMoneyElements.Clear();
        }

        public void AddMoneyElement(int moneyToAdd)
        {
            _addMoneyElements.Add(new AddMoneyElement()
            {
                MoneyReward = moneyToAdd
            });
        }
    }
}