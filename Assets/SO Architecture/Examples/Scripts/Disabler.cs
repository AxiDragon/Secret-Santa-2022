using UnityEngine;

namespace ScriptableObjectArchitecture.Examples
{
    public class Disabler : MonoBehaviour
    {
        [SerializeField] private GameObjectCollection _targetSet = default;

        public void DisableRandom()
        {
            if (_targetSet.Count > 0)
            {
                var index = Random.Range(0, _targetSet.Count);

                var objToDisable = _targetSet[index];
                objToDisable.SetActive(false);
            }
        }
    }
}