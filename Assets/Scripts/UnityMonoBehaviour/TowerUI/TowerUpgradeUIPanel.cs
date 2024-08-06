using System;
using Components;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UnityMonoBehaviour.TowerUI
{
    public class TowerUpgradeUIPanel : MonoBehaviour
    {
        [SerializeField] private Button _cancelUIButton;
        [SerializeField] private Button _upgradeTowerButton;

        [SerializeField] private TextMeshProUGUI _towerName;
        [SerializeField] private TextMeshProUGUI _damageText;
        [SerializeField] private TextMeshProUGUI _attSpeedText;
        [SerializeField] private TextMeshProUGUI _attRangeText;
        [SerializeField] private TextMeshProUGUI _upgradeText;
        
        private Entity _currentEntity;
        private TowerRegistryEntry _currentTowerState;
        private TowerRegistryEntry _upgradeTowerState;
        
        public void Display(Entity towerEntity, TowerRegistryEntry currentTower, TowerRegistryEntry nextTower, World world)
        {
            _currentEntity = towerEntity;
            _currentTowerState = currentTower;
            _upgradeTowerState = nextTower;
            
            var towerConfig = world.EntityManager.GetComponentData<TowerConfigAsset>(currentTower.TowerPrefab);
            
            _towerName.text = $"{towerConfig.Config.Value.TowerType} Lvl {towerConfig.Config.Value.Level}";
            _damageText.text = $"Damage: {towerConfig.Config.Value.ProjectileDamage}";
            _attSpeedText.text = $"Attack speed: {towerConfig.Config.Value.FireRate}";
            _attRangeText.text = $"Attack range: {towerConfig.Config.Value.FireRange}";
            
            _upgradeText.text = $"{nextTower.BuildPrice}g";
            gameObject.SetActive(true);
        }

        public void SetCallbacks(Action closeUICallback, Action<Entity,TowerRegistryEntry> replaceTowerCallback)
        {
            _cancelUIButton.onClick.RemoveAllListeners();
            _upgradeTowerButton.onClick.RemoveAllListeners();
            
            _cancelUIButton.onClick.AddListener(() => closeUICallback());
            _upgradeTowerButton.onClick.AddListener(() => replaceTowerCallback(_currentEntity ,_upgradeTowerState));
        }
    }
}