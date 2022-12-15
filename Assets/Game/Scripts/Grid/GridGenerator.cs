using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class GridGenerator : MonoBehaviour
{
    public GridPointListVariable grid;
    [SerializeField] int gridWidth = 5;
    [SerializeField] int gridLength = 5;
    [SerializeField] public float offset = 1f;
    [SerializeField] GridPoint basePoint;

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        grid.value.Clear();
        for (int i = 0; i < gridLength; i++)
        {
            List<GridPoint> list = new List<GridPoint>();
            for (int j = 0; j < gridWidth; j++)
            {
                Vector3 pos = new Vector3(i * offset, 0f, j * offset);
                GridPoint v = Instantiate(basePoint, pos, Quaternion.identity);
                v.name = $"Point {i} {j}";
                list.Add(v);
            }
            grid.value.Add(list);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Vector3 pos = new Vector3(i * offset, 0f, j * offset);
                Gizmos.DrawSphere(pos, .1f);
            }
        }
    }
}
