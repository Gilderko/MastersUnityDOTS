using Components;
using Components.Aspects;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;

namespace Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial class TowerPlacementSystem : SystemBase
    {
        private Camera _camera;
        private TowerPlacementLayersComponent _placementConfig;

        private CollisionFilter _filterMove;
        private CollisionFilter _filterPlace;
        private CollisionFilter _filterOverlap;
        
        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            CheckedStateRef.RequireForUpdate<PhysicsWorldSingleton>();
            CheckedStateRef.RequireForUpdate<TowerPlacementLayersComponent>();
            
            _camera = Camera.main;
        }

        protected override void OnStartRunning()
        {
            _placementConfig = SystemAPI.GetSingleton<TowerPlacementLayersComponent>();
            
            _filterMove = CollisionFilter.Default;
            _filterMove.BelongsTo = _placementConfig.BelongsToMove.Value;
            _filterMove.CollidesWith = _placementConfig.CollidesWithMove.Value;

            _filterPlace = CollisionFilter.Zero;
            _filterPlace.BelongsTo = _placementConfig.BelongsToPlacement.Value;
            _filterPlace.CollidesWith = _placementConfig.CollidesWithPlacement.Value;
            
            _filterOverlap = CollisionFilter.Zero;
            _filterOverlap.BelongsTo = _placementConfig.BelongsToOverlap.Value;
            _filterOverlap.CollidesWith = _placementConfig.CollidesWithOverlap.Value;
        }

        protected override void OnUpdate()
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var ecbBos = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(CheckedStateRef.WorldUnmanaged);
            
            if (!SystemAPI.TryGetSingletonEntity<TowerDummyComponent>(out var dummyTowerEntity))
            {
                return;
            }

            var dummyTower = SystemAPI.GetComponent<TowerDummyComponent>(dummyTowerEntity);
            GenerateInputRays(out var inputPlace, out var inputMove);
            
            if (!physicsWorld.CastRay(inputMove, out var moveHit))
            {
                return;
            }
                
            var newTransform = LocalTransform.Identity;
            newTransform.Position = moveHit.Position;
            ecbBos.SetComponent(dummyTowerEntity, newTransform);

            if (!SystemAPI.HasBuffer<Child>(dummyTower.Visual))
            {
                return;
            }
            
            var children = SystemAPI.GetBuffer<Child>(dummyTower.Visual);
            if (!physicsWorld.CastRay(inputPlace, out var placementHit))
            {  
                HandleInvalidTowerPosition(children, ecbBos, dummyTowerEntity);
            }
            else
            {
                var moneyStorageEntity = SystemAPI.GetSingletonEntity<MoneyComponent>();
                var moneyStorage = SystemAPI.GetAspect<MoneyStorageAspect>(moneyStorageEntity);

                var distances = new NativeList<DistanceHit>(Allocator.Temp);
                var placementHitLocalTransform = placementHit.Position;
                
                if (dummyTower.BuildPrice <= moneyStorage.CurrentMoney
                    && !physicsWorld.OverlapSphere(placementHitLocalTransform + math.up(), dummyTower.BuildRadius, ref distances, _filterOverlap))
                {
                    HandleTowerValidPosition(children);
                    PlaceNewTower(ecbBos, dummyTower, placementHitLocalTransform, moneyStorage, dummyTowerEntity);
                }
                else
                {
                    HandleInvalidTowerPosition(children, ecbBos, dummyTowerEntity);
                }

                distances.Dispose();
            }
        }

        private void HandleTowerValidPosition(DynamicBuffer<Child> visualChildren)
        {
            foreach (var visualChild in  visualChildren)
            {
                if (!SystemAPI.HasComponent<URPMaterialPropertyBaseColor>(visualChild.Value))
                {
                    continue;
                }
                var childColor = SystemAPI.GetComponentRW<URPMaterialPropertyBaseColor>(visualChild.Value);
                childColor.ValueRW.Value = new float4(0f, 1f, 0f, 0f);
            }
        }

        private void HandleInvalidTowerPosition(DynamicBuffer<Child> visualChildren, EntityCommandBuffer ecbBos, Entity dummyTowerEntity)
        {
            foreach (var visualChild in visualChildren)
            {
                if (!SystemAPI.HasComponent<URPMaterialPropertyBaseColor>(visualChild.Value))
                {
                    continue;
                }
                var childColor = SystemAPI.GetComponentRW<URPMaterialPropertyBaseColor>(visualChild.Value);
                childColor.ValueRW.Value = new float4(1f, 0f, 0f, 0f);
            }
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            ecbBos.DestroyEntity(dummyTowerEntity);
        }

        private void PlaceNewTower(EntityCommandBuffer ecbBos, TowerDummyComponent dummyTower, float3 placementHitLocalTransform, MoneyStorageAspect moneyStorage,
            Entity dummyTowerEntity)
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            
            var newTower = ecbBos.Instantiate(dummyTower.TowerPrefab);
            var transform = LocalTransform.Identity;
            transform.Position = placementHitLocalTransform;
            ecbBos.SetComponent(newTower, transform);
            moneyStorage.AddMoneyElement(-dummyTower.BuildPrice);

            ecbBos.DestroyEntity(dummyTowerEntity);
        }

        private void GenerateInputRays(out RaycastInput inputPlace, out RaycastInput inputMove)
        {
            var screenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(new Vector2(screenPosition.x, screenPosition.y));
                
            inputMove= new RaycastInput()
            {
                Start = ray.origin,
                Filter = _filterMove,
                End = ray.GetPoint(_camera.farClipPlane)
            };

            inputPlace = new RaycastInput()
            {
                Start = ray.origin,
                Filter = _filterPlace,
                End = ray.GetPoint(_camera.farClipPlane)
            };
        }
    }
}