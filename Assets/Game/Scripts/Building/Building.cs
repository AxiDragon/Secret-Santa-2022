using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Building : MonoBehaviour
{
    public bool isPreview = false;
    [HideInInspector] public float buildingOffset;

    private void Awake()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        buildingOffset = transform.position.y - bounds.min.y + .001f;
    }
}
