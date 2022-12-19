using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BuildingConstructor : MonoBehaviour
{
    [SerializeField] private Material previewMaterial;
    [SerializeField] private GridPointListVariable gridPointList;
    [HideInInspector] public Building currentBuilding;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private UnityEvent activateBuildMode;
    [SerializeField] private UnityEvent deActivateBuildMode;
    [HideInInspector] public bool buildModeActive;
    private GridPoint closestGridPoint;
    private Building previewBuilding;

    public bool BuildModeActive
    {
        get => buildModeActive;
        set
        {
            buildModeActive = value;

            if (buildModeActive)
            {
                activateBuildMode?.Invoke();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                ClosestGridPoint = gridPointList.GetClosestGridPoint(GetMousePosition());
                GeneratePreview(currentBuilding);
            }
            else
            {
                deActivateBuildMode?.Invoke();
                Cursor.lockState = CursorLockMode.Locked;

                if (previewBuilding != null)
                    Destroy(previewBuilding.gameObject);
            }
        }
    }

    private GridPoint ClosestGridPoint
    {
        get => closestGridPoint;
        set
        {
            var currentValue = closestGridPoint;
            closestGridPoint = value;

            if (!buildModeActive)
                return;

            if (currentValue != value)
                if (previewBuilding != null)
                    MovePreview(closestGridPoint);
        }
    }

    private void Update()
    {
        if (buildModeActive)
            ClosestGridPoint = gridPointList.GetClosestGridPoint(GetMousePosition());
    }

    public void ToggleBuildModeInput(InputAction.CallbackContext callback)
    {
        if (callback.performed && currentBuilding)
            BuildModeActive = !BuildModeActive;
    }

    private Vector3 GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 500f);

        if (Physics.Raycast(ray, out hit, 500f, raycastMask.value)) return hit.point;

        return transform.position;
    }

    public void Build()
    {
        var buildingInstance = Instantiate(currentBuilding, ClosestGridPoint.transform);
        buildingInstance.AddPointsToGrid();

        ResetBuildMode();

        SpawnObjectEasing(buildingInstance.gameObject);
    }

    private void ResetBuildMode()
    {
        currentBuilding = null;
        BuildModeActive = false;
    }

    private static void SpawnObjectEasing(GameObject buildingInstance)
    {
        var baseScale = buildingInstance.transform.localScale.x;
        buildingInstance.transform.localScale = Vector3.zero;
        buildingInstance.transform.DOScale(baseScale, .5f).SetEase(Ease.OutBack);
    }

    public void GeneratePreview(Building building)
    {
        previewBuilding = Instantiate(building, ClosestGridPoint.transform);
        previewBuilding.isPreview = true;
        previewBuilding.gameObject.layer = LayerMask.NameToLayer("Preview");

        SpawnObjectEasing(previewBuilding.gameObject);
        DisableColliders(previewBuilding.gameObject);
        ApplyPreviewMaterials(previewBuilding.gameObject);
    }

    private void DisableColliders(GameObject gameObject)
    {
        foreach (var collider in gameObject.GetComponents<Collider>()) collider.enabled = false;

        foreach (var collider in gameObject.GetComponentsInChildren<Collider>()) collider.enabled = false;
    }

    public void MovePreview(GridPoint point)
    {
        var buildingTransform = previewBuilding.transform;
        buildingTransform.parent = point.transform;
        buildingTransform.localPosition = Vector3.zero;
    }

    private void ApplyPreviewMaterials(GameObject gameObject)
    {
        var renderers = gameObject.GetComponentsInChildren<Renderer>().ToList();

        if (gameObject.TryGetComponent(out Renderer baseRenderer)) renderers.Add(baseRenderer);

        for (var i = 0; i < renderers.Count; i++)
        {
            var previewMaterials = new Material[renderers[i].materials.Length];

            for (var j = 0; j < previewMaterials.Length; j++) previewMaterials[j] = previewMaterial;

            renderers[i].materials = previewMaterials;
        }
    }
}