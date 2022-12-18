using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Building : MonoBehaviour
{
    public bool isPreview;
    private bool pointsAdded = false;
    [HideInInspector] public float buildingOffset;
    [SerializeField] private GridPointListVariable gridPointList;

    private void Awake()
    {
        var bounds = GetComponent<Collider>().bounds;
        buildingOffset = transform.position.y - bounds.min.y + .001f;
    }

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