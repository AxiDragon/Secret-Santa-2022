using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GridPointListVariable grid;
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridLength = 5;
    [SerializeField] public float offset = 1f;
    [SerializeField] private GridPoint basePoint;

    private void Awake()
    {
        GenerateGrid();
    }

    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        for (var i = 0; i < gridLength; i++)
        for (var j = 0; j < gridWidth; j++)
        {
            var pos = new Vector3(i * offset, 0f, j * offset);
            Gizmos.DrawSphere(pos, .1f);
        }
    }

    private void GenerateGrid()
    {
        grid.value.Clear();
        for (var i = 0; i < gridLength; i++)
        {
            var list = new List<GridPoint>();
            for (var j = 0; j < gridWidth; j++)
            {
                var pos = new Vector3(i * offset, 0f, j * offset);
                var v = Instantiate(basePoint, pos, Quaternion.identity);
                v.name = $"Point {i} {j}";
                list.Add(v);
            }

            grid.value.Add(list);
        }
    }
}