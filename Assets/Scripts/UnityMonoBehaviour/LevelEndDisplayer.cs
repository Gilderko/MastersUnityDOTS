using System;
using Components;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace UnityMonoBehaviour
{
    public class LevelEndDisplayer : MonoBehaviour
    {
        [SerializeField] private GameObject[] _panelsToDeactivate = new GameObject[] {};
        
        [SerializeField] private GameObject _finalPanel;
        [SerializeField] private TextMeshProUGUI _text;
        
        private World _world;
        private LevelEndComponent _levelEndComponent;
        
        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
        }

        private void Update()
        {
            var success = _world.EntityManager.CreateEntityQuery(typeof(LevelEndComponent)).TryGetSingleton<LevelEndComponent>(out _levelEndComponent);

            if (!success)
            {
                return;
            }

            foreach (var panel in _panelsToDeactivate)
            {
                panel.SetActive(false);
            }
            
            _text.text = _levelEndComponent.Success ? "Level successfully completed" : "Level failed";
            _finalPanel.SetActive(true);
            
            gameObject.SetActive(false);
        }
    }
}