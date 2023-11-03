using Components;
using Components.Aspects;
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

            var currentColor = SystemAPI.GetComponentRW<URPMaterialPropertyBaseColor>(dummyTower.Visual);
            if(!physicsWorld.CastRay(inputPlace, out var placementHit))
            {  
                currentColor.ValueRW.Value = new float4(1f, 0f, 0f,0f);
            }
            else
            {
                var moneyStorageEntity = SystemAPI.GetSingletonEntity<MoneyComponent>();
                var moneyStorage = SystemAPI.GetAspect<MoneyStorageAspect>(moneyStorageEntity);
                currentColor.ValueRW.Value = new float4(0f, 1f, 0f,0f);
                
                if (Input.GetMouseButtonDown(0) && dummyTower.BuildPrice < moneyStorage.CurrentMoney)
                {
                    var placementHitLocalTransform = SystemAPI.GetComponent<LocalTransform>(placementHit.Entity);
            
                    var newTower = ecbBos.Instantiate(dummyTower.TowerPrefab);
                    var transform = LocalTransform.Identity;
                    transform.Position = placementHitLocalTransform.Position + new float3(0f,2f,0f);
                    ecbBos.SetComponent(newTower, transform);
                    moneyStorage.AddMoneyElement(-dummyTower.BuildPrice);
                    
                    ecbBos.DestroyEntity(dummyTowerEntity);
                    return;
                }
            }

            if (!physicsWorld.CastRay(inputMove, out var moveHit))
            {
                return;
            }
                
            var newTransform = LocalTransform.Identity;
            newTransform.Position = moveHit.Position + new float3(0f,2f,0f);
            ecbBos.SetComponent(dummyTowerEntity, newTransform);
        }

        private void GenerateInputRays(out RaycastInput inputPlace, out RaycastInput inputMove)
        {
            var screenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(new Vector2(screenPosition.x, screenPosition.y));
            
            var moveFilter = CollisionFilter.Default;
            moveFilter.BelongsTo = _placementConfig.BelongsToMove.Value;
            moveFilter.CollidesWith = _placementConfig.CollidesWithMove.Value;

            var placementFilter = CollisionFilter.Zero;
            placementFilter.BelongsTo = _placementConfig.BelongsToPlacement.Value;
            placementFilter.CollidesWith = _placementConfig.CollidesWithPlacement.Value;
                
            inputMove= new RaycastInput()
            {
                Start = ray.origin,
                Filter = moveFilter,
                End = ray.GetPoint(_camera.farClipPlane)
            };

            inputPlace = new RaycastInput()
            {
                Start = ray.origin,
                Filter = placementFilter,
                End = ray.GetPoint(_camera.farClipPlane)
            };
        }
    }
}