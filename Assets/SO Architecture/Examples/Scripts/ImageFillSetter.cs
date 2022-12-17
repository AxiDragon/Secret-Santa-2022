using UnityEngine;
using UnityEngine.UI;

namespace ScriptableObjectArchitecture.Examples
{
    public class ImageFillSetter : MonoBehaviour
    {
        [SerializeField] private FloatReference _variable = default;

        [SerializeField] private FloatReference _maxValue = default;

        [SerializeField] private Image _imageTarget = default;

        private void Update()
        {
            _imageTarget.fillAmount = Mathf.Clamp01(_variable.Value / _maxValue.Value);
        }
    }
}