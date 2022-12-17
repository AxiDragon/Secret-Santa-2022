using UnityEngine;

namespace ScriptableObjectArchitecture.Examples
{
    public class DamageDealerWithEvent : DamageDealer
    {
        [SerializeField] private GameEvent _onDamagedEvent = default;

        protected override void DealDamage(UnitHealth target)
        {
            base.DealDamage(target);

            _onDamagedEvent.Raise();
        }
    }
}