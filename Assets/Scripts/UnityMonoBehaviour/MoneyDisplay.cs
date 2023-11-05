using System;
using Components;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace UnityMonoBehaviour
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private Entity _moneyStorageEntity;
        private EntityManager _entityManager;
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _moneyStorageEntity = _entityManager.CreateEntityQuery(typeof(MoneyComponent)).GetSingletonEntity();
        }

        private void Update()
        {
            var moneyComponent = _entityManager.GetComponentData<MoneyComponent>(_moneyStorageEntity);
            _text.text = $"Gold: {moneyComponent.CurrentMoney}";
        }
    }
}