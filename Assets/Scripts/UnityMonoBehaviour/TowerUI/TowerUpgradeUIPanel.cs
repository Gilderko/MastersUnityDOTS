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
        
        public void Display(Entity towerEntity, TowerRegistryEntry currentTower, TowerRegistryEntry nextTower)
        {
            _currentEntity = towerEntity;
            _currentTowerState = currentTower;
            _upgradeTowerState = nextTower;

            _towerName.text = $"{currentTower.Type} Lvl {currentTower.Level}";
            _damageText.text = $"Damage: {currentTower}";
            _attSpeedText.text = $"Attack speed: {currentTower.Config.Value.FireRate}";
            _attRangeText.text = $"Attack range: {currentTower.Config.Value.FireRange}";
            
            _upgradeText.text = $"Upgrade: {nextTower.BuildPrice} gold";
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