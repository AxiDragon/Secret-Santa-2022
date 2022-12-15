using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GridPointListVariable : ScriptableObject
{
    public List<List<GridPoint>> value = new List<List<GridPoint>>();

    public GridPoint GetClosestGridPoint(Vector3 checkPoint, bool includeOccupied = false)
    {

        float closest = Mathf.Infinity;
        GridPoint closestGridVertex = null;

        for (int i = 0; i < value.Count; i++)
        {
            for (int j = 0; j < value[i].Count; j++)
            {
                if (!includeOccupied && IsOccupiedWithBuilding(value[i][j]))
                    continue;

                if (Vector3.Distance(checkPoint, value[i][j].transform.position) < closest)
                {
                    closestGridVertex = value[i][j];
                    closest = Vector3.Distance(checkPoint, value[i][j].transform.position);
                }
            }
        }

        return closestGridVertex;
    }

    public bool IsOccupiedWithBuilding(GridPoint gridPoint)
    {
        foreach(Building building in gridPoint.GetComponentsInChildren<Building>())
        {
            if (!building.isPreview)
                return true;
        }

        return false;
    }
}
