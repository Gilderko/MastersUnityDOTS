using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial class TowerUpgradesSystem : SystemBase
    {
        public event Action<Entity, TowerRegistryEntry, TowerRegistryEntry> OnDisplayTowerUIEvent;
        
        private NativeParallelMultiHashMap<int, TowerRegistryEntry> _towerLevels; 
        
        private Camera _camera;
        private TowerPlacementLayersComponent _placementConfig;
        private CollisionFilter _filterTower;

        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<PhysicsWorldSingleton>();
            CheckedStateRef.RequireForUpdate<TowerPlacementLayersComponent>();
            
            _camera = Camera.main;
        }

        protected override void OnStartRunning()
        {
            // Get the registry + create hashmap
            var towers = SystemAPI.GetSingletonBuffer<TowerRegistryEntry>();
            _towerLevels = new NativeParallelMultiHashMap<int, TowerRegistryEntry>(towers.Capacity,Allocator.Persistent);
            
            foreach (var tower in towers)
            {
                _towerLevels.Add((int) tower.Config.Value.TowerType, tower);
            }
            
            // Get the layers config
            _placementConfig = SystemAPI.GetSingleton<TowerPlacementLayersComponent>();
            
            _filterTower = CollisionFilter.Zero;
            _filterTower.BelongsTo = _placementConfig.BelongsToOverlap.Value;
            _filterTower.CollidesWith = _placementConfig.CollidesWithOverlap.Value;
        }

        protected override void OnUpdate()
        {
            if (!Input.GetMouseButtonDown(0))
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
            
            if (!physicsWorld.CastRay(inputMove, out var moveHit))
            {
                return;
            }

            var towerConfig = SystemAPI.GetComponent<TowerConfigAsset>(moveHit.Entity).Config.Value;

            TowerRegistryEntry? currentTower = null;
            TowerRegistryEntry? currentTowerUpgrade = null;
            foreach (var towerRegEntry in _towerLevels.GetValuesForKey((int) towerConfig.TowerType))
            {
                if (towerRegEntry.Config.Value.Level == towerConfig.Level)
                {
                    currentTower = towerRegEntry;
                }
                
                if (towerRegEntry.Config.Value.Level == towerConfig.Level + 1)
                {
                    currentTowerUpgrade = towerRegEntry;
                }
            }

            if (!currentTower.HasValue || !currentTowerUpgrade.HasValue)
            {
                return;
            }
            
            OnDisplayTowerUIEvent?.Invoke(moveHit.Entity, currentTower.Value, currentTowerUpgrade.Value);
        }
    }
}