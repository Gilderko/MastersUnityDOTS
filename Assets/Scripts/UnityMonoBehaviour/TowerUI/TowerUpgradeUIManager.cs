using System;
using Components;
using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UnityMonoBehaviour.TowerUI
{
    public class TowerUpgradeUIManager : MonoBehaviour
    {
        [SerializeField] private TowerUpgradeUIPanel _towerUpgradePrefab;
        [SerializeField] private float _panelYOffset = 10.0f;
        
        private TowerUpgradeUIPanel _currentTowerUpgradePanel;
        
        private World _world;
        private TowerUpgradesSystem _towerUpgradesSystem;
        private Entity _moneyStorageEntity;

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _towerUpgradesSystem = _world.GetExistingSystemManaged<TowerUpgradesSystem>();
            _moneyStorageEntity = _world.EntityManager.CreateEntityQuery(typeof(MoneyComponent))
                .GetSingletonEntity();
            
            if (_towerUpgradesSystem == null)
            {
                return;
            }
            
            _towerUpgradesSystem.OnDisplayTowerUIEvent += DisplayTowerUpgradeUI;
        }

        private void OnDisable()
        {
            _towerUpgradesSystem.OnDisplayTowerUIEvent -= DisplayTowerUpgradeUI;
        }

        private void DisplayTowerUpgradeUI(Entity towerEntity, TowerRegistryEntry currentTower, TowerRegistryEntry towerUpgrade)
        {
            // Create UI element if it doesnt exist yet
            // change its content to correspond with entity
            // add 2 callbacks one to close UI and one to upgrade tower
            if (_currentTowerUpgradePanel == null)
            {
                _currentTowerUpgradePanel = Instantiate(_towerUpgradePrefab);
            }

            var transform = _world.EntityManager.GetComponentData<LocalTransform>(towerEntity);
            _currentTowerUpgradePanel.transform.position = (Vector3) transform.Position + Vector3.up * _panelYOffset;
            _currentTowerUpgradePanel.Display(towerEntity, currentTower, towerUpgrade);
            _currentTowerUpgradePanel.SetCallbacks(CloseUICallback, ReplaceTowerCallback);
        }

        private void CloseUICallback()
        {
            _currentTowerUpgradePanel.gameObject.SetActive(false);
        }

        private void ReplaceTowerCallback(Entity towerEntity, TowerRegistryEntry upgradeTower)
        {
            var currentMoney = _world.EntityManager.GetComponentData<MoneyComponent>(_moneyStorageEntity);
            if (currentMoney.CurrentMoney <= upgradeTower.BuildPrice)
            {
                return;
            }
            
            var positionOld = _world.EntityManager.GetComponentData<LocalTransform>(towerEntity);
            _world.EntityManager.DestroyEntity(towerEntity);
            
            var upgradedTower = _world.EntityManager.Instantiate(upgradeTower.TowerPrefab);
            _world.EntityManager.SetComponentData(upgradedTower, positionOld);
            _world.EntityManager.SetComponentData(_moneyStorageEntity, new MoneyComponent()
            {
                CurrentMoney = currentMoney.CurrentMoney - upgradeTower.BuildPrice
            });

            CloseUICallback();
        }
    }
}