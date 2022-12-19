using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridPointListVariable grid;

    private void Awake()
    {
        grid.value.Clear();
    }
}
