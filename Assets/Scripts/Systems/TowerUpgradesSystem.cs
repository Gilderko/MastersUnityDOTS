using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Systems
{
    /// <summary>
    /// Processes input information from the user and signals for the instantiation of upgrade UI for a tower
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial class TowerUpgradesSystem : SystemBase
    {
        public event Action<Entity, TowerRegistryEntry, TowerRegistryEntry> OnDisplayTowerUIEvent;
        
        private NativeParallelMultiHashMap<int, TowerRegistryEntry> _towerLevels; 
        
        private Camera _camera;
        private EventSystem _eventSystem;
        private TowerPlacementLayersComponent _placementConfig;
        private CollisionFilter _filterTower;

        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<PhysicsWorldSingleton>();
            CheckedStateRef.RequireForUpdate<TowerPlacementLayersComponent>();
        }

        protected override void OnStartRunning()
        {
            var towers = SystemAPI.GetSingletonBuffer<TowerRegistryEntry>();
            _towerLevels = new NativeParallelMultiHashMap<int, TowerRegistryEntry>(towers.Capacity,Allocator.Persistent);
            
            foreach (var tower in towers)
            {
                _towerLevels.Add((int) tower.TowerType, tower);
            }
            
            // Get the layers config
            _placementConfig = SystemAPI.GetSingleton<TowerPlacementLayersComponent>();
            
            _filterTower = CollisionFilter.Zero;
            _filterTower.BelongsTo = _placementConfig.BelongsToOverlap.Value;
            _filterTower.CollidesWith = _placementConfig.CollidesWithOverlap.Value;
            
            _camera = Camera.main;
            _eventSystem = EventSystem.current;
        }

        protected override void OnDestroy()
        {
            _towerLevels.Dispose();
        }

        protected override void OnUpdate()
        {
            if (!Input.GetMouseButtonDown(0) || _eventSystem.IsPointerOverGameObject())
            {
                return;
            }
            
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var screenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(new Vector2(screenPosition.x, screenPosition.y));
                
            var inputMove = new RaycastInput()
            {
                Start = ray.origin,
                Filter = _filterTower,
                End = ray.GetPoint(_camera.farClipPlane)
            };
            
            if (!physicsWorld.CastRay(inputMove, out var towerHit))
            {
                return;
            }
            
            var towerConfig = SystemAPI.GetComponent<TowerConfigAsset>(towerHit.Entity).Config.Value;
            
            TowerRegistryEntry? currentTower = null;
            TowerRegistryEntry? currentTowerUpgrade = null;
            foreach (var towerRegEntry in _towerLevels.GetValuesForKey((int) towerConfig.TowerType))
            {
                if (towerRegEntry.TowerLevel == towerConfig.Level)
                {
                    currentTower = towerRegEntry;
                }
                
                if (towerRegEntry.TowerLevel == towerConfig.Level + 1)
                {
                    currentTowerUpgrade = towerRegEntry;
                }
            }

            if (!currentTower.HasValue || !currentTowerUpgrade.HasValue)
            {
                return;
            }
            
            OnDisplayTowerUIEvent?.Invoke(towerHit.Entity, currentTower.Value, currentTowerUpgrade.Value);
        }
    }
}