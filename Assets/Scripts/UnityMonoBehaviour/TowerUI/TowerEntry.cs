using Authoring;
using Components;
using UnityEngine;

namespace UnityMonoBehaviour.TowerUI
{
    [System.Serializable]
    public class TowerEntry
    {
        public int BuildPrice;
        public float BuildRadius;

        public int Level = 0;
        public bool Buildable = false;
        public TowerType Type = TowerType.Normal;
        
        public TowerAuthoring TowerPrefab;
        public DummyTowerAuthoring DummyTowerPrefab;
    }
}