using UnityEngine;

public class Building : MonoBehaviour
{
    [HideInInspector] public bool isPreview;
    private bool pointsAdded = false;
    [SerializeField] private GridPointListVariable gridPointList;

    public void AddPointsToGrid()
    {
        if (pointsAdded)
            return;
        
        pointsAdded = true;
        
        foreach (var gridPoint in GetComponentsInChildren<GridPoint>())
        {
            gridPointList.value[0].Add(gridPoint);
        }
    }
}