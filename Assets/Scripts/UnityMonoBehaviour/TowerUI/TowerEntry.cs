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
        public bool Buildable = false;
        
        public TowerAuthoring TowerPrefab;
        public DummyTowerAuthoring DummyTowerPrefab;
    }
}