using UnityEngine;

namespace UnityMonoBehaviour.SpawningConfig
{
    [System.Serializable]
    public class SpawnTransformModifier
    {
        [SerializeField] private Vector3 _positionSpawnOffset = Vector3.zero;
        [SerializeField] private float _scaleMultiplication = 1.0f;
        [SerializeField] private Vector3 _rotationSpawnOffset = Vector3.zero;

        public Vector3 PositionSpawnOffset
        {
            get => _positionSpawnOffset;
            set => _positionSpawnOffset = value;
        }

        public float ScaleMultiplication
        {
            get => _scaleMultiplication;
            set => _scaleMultiplication = value;
        }

        public Vector3 RotationOffset
        {
            get => _rotationSpawnOffset;
            set => _rotationSpawnOffset = value;
        }
    }
}