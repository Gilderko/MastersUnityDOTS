using System;
using UnityEngine;

namespace UnityMonoBehaviour
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _scrollSpeed;
        
        private void Update()
        {
            var moveVector = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                moveVector.z += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveVector.x -= 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveVector.z -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveVector.x += 1;
            }
            
            moveVector.y += -1 * Input.mouseScrollDelta.y * _scrollSpeed;

            _mainCamera.transform.position += moveVector * (_moveSpeed * Time.deltaTime);
        }
    }
}