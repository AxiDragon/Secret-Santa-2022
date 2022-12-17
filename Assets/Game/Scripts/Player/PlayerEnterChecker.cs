using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerEnterChecker : MonoBehaviour
{
    [HideInInspector] public bool playerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerEntered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerEntered = false;
    }
}