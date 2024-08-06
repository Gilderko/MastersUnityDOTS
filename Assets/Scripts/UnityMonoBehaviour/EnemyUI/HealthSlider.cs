using UnityEngine;
using UnityEngine.UI;

namespace UnityMonoBehaviour
{
    public class HealthSlider : MonoBehaviour
    {
        [SerializeField] private Image _healthForeground;

        public void DisplayHealth(float healthFraction)
        {
            _healthForeground.fillAmount = healthFraction;
        }
    }
}