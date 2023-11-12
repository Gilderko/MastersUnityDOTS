using System;
using UnityEngine;

namespace UnityMonoBehaviour
{
    public class FaceCameraUI : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            transform.forward = _mainCamera.transform.forward;
        }
    }
}