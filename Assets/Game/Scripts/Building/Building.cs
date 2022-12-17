using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Building : MonoBehaviour
{
    public bool isPreview;
    [HideInInspector] public float buildingOffset;

    private void Awake()
    {
        var bounds = GetComponent<Collider>().bounds;
        buildingOffset = transform.position.y - bounds.min.y + .001f;
    }
}