using UnityEngine;

public class UnparentOnGameStart : MonoBehaviour
{
    private void Awake()
    {
        transform.parent = null;
    }
}
