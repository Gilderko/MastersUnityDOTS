using UnityEngine;

namespace UnityMonoBehaviour.TowerPlacementConfig
{
    [System.Serializable]
    public class TowerEntry
    {
        public int BuildPrice;
        
        public GameObject TowerPrefab;
        public GameObject DummyTowerPrefab;
    }
}