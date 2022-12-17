using UnityEngine;
using UnityEngine.UI;

namespace ScriptableObjectArchitecture.Examples
{
    public class CollectionCountDisplayer : MonoBehaviour
    {
        [SerializeField] private Text _textTarget = default;

        [SerializeField] private BaseCollection _setTarget = default;

        [SerializeField] private string _textFormat = "There are {0} things.";

        private void Update()
        {
            _textTarget.text = string.Format(_textFormat, _setTarget.Count);
        }
    }
}