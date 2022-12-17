using UnityEngine;

namespace ScriptableObjectArchitecture.Examples
{
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] private FloatReference _damageAmount = default;

        private void OnTriggerEnter(Collider other)
        {
            var targetHealth = other.gameObject.GetComponent<UnitHealth>();

            if (targetHealth != null)
                DealDamage(targetHealth);
        }

        protected virtual void DealDamage(UnitHealth target)
        {
            target.Health.Value -= _damageAmount.Value;
        }
    }
}