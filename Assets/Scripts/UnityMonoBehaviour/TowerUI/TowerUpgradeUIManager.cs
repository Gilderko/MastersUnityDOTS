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

        private TowerUpgradeUIPanel _currentTowerUpgradePanel;
        
        private World _world;
        private TowerUpgradesSystem _towerUpgradesSystem;

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _towerUpgradesSystem = _world.EntityManager.CreateEntityQuery(typeof(TowerUpgradesSystem)
            ).GetSingleton<TowerUpgradesSystem>();

            if (_towerUpgradesSystem == null)
            {
                return;
            }
            
            _towerUpgradesSystem.OnDisplayTowerUIEvent += DisplayTowerUpgradeUI;
        }

        private void OnEnable()
        {
            if (_world is not { IsCreated: true } || _towerUpgradesSystem == null)
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

            _currentTowerUpgradePanel.Display(towerEntity, currentTower, towerUpgrade);
            _currentTowerUpgradePanel.SetCallbacks(CloseUICallback, ReplaceTowerCallback);
        }

        private void CloseUICallback()
        {
            _currentTowerUpgradePanel.gameObject.SetActive(false);
        }

        private void ReplaceTowerCallback(Entity towerEntity, TowerRegistryEntry upgradeTower)
        {
            var positionOld = _world.EntityManager.GetComponentData<LocalTransform>(towerEntity);
            _world.EntityManager.DestroyEntity(towerEntity);
            
            var upgradedTower = _world.EntityManager.Instantiate(upgradeTower.TowerPrefab);
            _world.EntityManager.SetComponentData(upgradedTower, positionOld);
        }
    }
}