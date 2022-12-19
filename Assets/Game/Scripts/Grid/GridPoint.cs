using System;
using UnityEngine;

public class GridPoint : MonoBehaviour
{
    [SerializeField] private GridPointListVariable grid;
    public bool automaticallyAddToGrid = true;

    private void Start()
    {
        // if (automaticallyAddToGrid)
        //     grid.value[0].Add(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .1f);
    }
}