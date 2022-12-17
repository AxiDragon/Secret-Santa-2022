using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GridPointListVariable : ScriptableObject
{
    public List<List<GridPoint>> value = new();

    public GridPoint GetClosestGridPoint(Vector3 checkPoint, bool includeOccupied = false)
    {
        var closest = Mathf.Infinity;
        GridPoint closestGridVertex = null;

        for (var i = 0; i < value.Count; i++)
        for (var j = 0; j < value[i].Count; j++)
        {
            if (!includeOccupied && IsOccupiedWithBuilding(value[i][j]))
                continue;

            if (Vector3.Distance(checkPoint, value[i][j].transform.position) < closest)
            {
                closestGridVertex = value[i][j];
                closest = Vector3.Distance(checkPoint, value[i][j].transform.position);
            }
        }

        return closestGridVertex;
    }

    public bool IsOccupiedWithBuilding(GridPoint gridPoint)
    {
        foreach (var building in gridPoint.GetComponentsInChildren<Building>())
            if (!building.isPreview)
                return true;

        return false;
    }
}