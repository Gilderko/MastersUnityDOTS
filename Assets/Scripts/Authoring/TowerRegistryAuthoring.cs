﻿using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityMonoBehaviour.TowerPlacementConfig;

namespace Authoring
{
    public class TowerRegistryAuthoring : MonoBehaviour
    {
        public List<TowerEntry> Towers;
        
        public PhysicsCategoryTags BelongsToPlacement;
        public PhysicsCategoryTags CollidesWithPlacement;
        
        public PhysicsCategoryTags BelongsToMove;
        public PhysicsCategoryTags CollidesWithMove;
        
        public PhysicsCategoryTags BelongsToOverlap;
        public PhysicsCategoryTags CollidesWithOverlap;
        
        private class TowerRegistryAuthoringBaker : Baker<TowerRegistryAuthoring>
        {
            public override void Bake(TowerRegistryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<TowerRegistryEntry>(entity);

                AddComponent(entity, new TowerPlacementLayersComponent()
                {
                    BelongsToMove = authoring.BelongsToMove,
                    CollidesWithMove = authoring.CollidesWithMove,
                    
                    BelongsToPlacement = authoring.BelongsToPlacement,
                    CollidesWithPlacement = authoring.CollidesWithPlacement,
                    
                    BelongsToOverlap = authoring.BelongsToOverlap,
                    CollidesWithOverlap = authoring.CollidesWithOverlap
                });
                
                foreach (var towerToAdd in authoring.Towers)
                {
                    buffer.Add(new TowerRegistryEntry()
                    {
                        DummyPrefab = GetEntity(towerToAdd.DummyTowerPrefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
}