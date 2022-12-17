using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrewer : MonoBehaviour
{
    [SerializeField] private float brewingRange = 2.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, brewingRange);
    }

    public void BrewInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) AttemptBrew();
    }

    private void AttemptBrew()
    {
    }
}