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
using RaycastHit = Unity.Physics.RaycastHit;

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

            if (!SystemAPI.HasBuffer<Child>(dummyTower.Visual))
            {
                return;
            }

            var isValidTowerPosition = physicsWorld.CastRay(inputPlace, out var placementHit);
            var placementHitLocalTransform = placementHit.Position;
            var distances = new NativeList<DistanceHit>(Allocator.Temp);
            isValidTowerPosition = isValidTowerPosition && !physicsWorld.OverlapSphere(placementHitLocalTransform + math.up(), dummyTower.BuildRadius, ref distances, _filterOverlap);

            if (Input.GetMouseButtonDown(0))
            {
                HandleTowerBuild(dummyTower, isValidTowerPosition, ecbBos, placementHitLocalTransform, dummyTowerEntity);
            }
            else
            {
                HandleTowerMovement(ecbBos, dummyTowerEntity, dummyTower, moveHit, isValidTowerPosition);
            }
            
            distances.Dispose();
        }

        private void HandleTowerMovement(EntityCommandBuffer ecbBos, Entity dummyTowerEntity, TowerDummyComponent dummyTower, RaycastHit moveHit, bool isValidTowerPosition)
        {
            MoveEntityToPlace(moveHit, ecbBos, dummyTowerEntity);

            var visualChildren = SystemAPI.GetBuffer<Child>(dummyTower.Visual);
            foreach (var visualChild in visualChildren)
            {
                if (!SystemAPI.HasComponent<URPMaterialPropertyBaseColor>(visualChild.Value))
                {
                    continue;
                }

                var childColor = SystemAPI.GetComponentRW<URPMaterialPropertyBaseColor>(visualChild.Value);
                childColor.ValueRW.Value = new float4(isValidTowerPosition ? 0 : 1, isValidTowerPosition ? 1 : 0, 0f, 0f);
            }
        }

        private void HandleTowerBuild(TowerDummyComponent dummyTower, bool isValidTowerPosition, EntityCommandBuffer ecbBos, float3 placementHitLocalTransform, Entity dummyTowerEntity)
        {
            var moneyStorageEntity = SystemAPI.GetSingletonEntity<MoneyComponent>();
            var moneyStorage = SystemAPI.GetAspect<MoneyStorageAspect>(moneyStorageEntity);
            
            if (dummyTower.BuildPrice <= moneyStorage.CurrentMoney && isValidTowerPosition)
            {
                PlaceNewTower(ecbBos, dummyTower, placementHitLocalTransform, moneyStorage, dummyTowerEntity);
            }
            else
            {
                ecbBos.DestroyEntity(dummyTowerEntity);
            }

        }

        private static void MoveEntityToPlace(RaycastHit moveHit, EntityCommandBuffer ecbBos, Entity dummyTowerEntity)
        {
            var newTransform = LocalTransform.Identity;
            newTransform.Position = moveHit.Position;
            ecbBos.SetComponent(dummyTowerEntity, newTransform);
        }

        private void PlaceNewTower(EntityCommandBuffer ecbBos, TowerDummyComponent dummyTower, float3 placementHitLocalTransform, MoneyStorageAspect moneyStorage,
            Entity dummyTowerEntity)
        {
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