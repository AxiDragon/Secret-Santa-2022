using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BuildingConstructor : MonoBehaviour
{
    [SerializeField] private Material previewMaterial;
    [SerializeField] private GridPointListVariable gridPointList;
    [SerializeField] private Building testBuilding;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private UnityEvent activateBuildMode;
    [SerializeField] private UnityEvent deActivateBuildMode;
    private bool buildModeActive;
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
                GeneratePreview(testBuilding);
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
        if (callback.action.WasPerformedThisFrame())
            BuildModeActive = !BuildModeActive;
    }

    private Vector3 GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 500f);

        if (Physics.Raycast(ray, out hit, 500f, raycastMask.value))
        {
            print(hit.collider.name);
            return hit.point;
        }

        return transform.position;
    }

    public void BuildInput(InputAction.CallbackContext callback)
    {
        if (callback.action.WasPerformedThisFrame() && ClosestGridPoint != null)
        {
            Destroy(previewBuilding.gameObject);
            Build(testBuilding);
        }
    }

    public void Build(Building building)
    {
        var buildingInstance = Instantiate(building, ClosestGridPoint.transform);
        buildingInstance.transform.localPosition = Vector3.up * buildingInstance.buildingOffset;
        SpawnObjectEasing(buildingInstance.gameObject);
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
        previewBuilding.transform.localPosition = Vector3.up * previewBuilding.buildingOffset;
        previewBuilding.isPreview = true;
        SpawnObjectEasing(previewBuilding.gameObject);
        ApplyPreviewMaterials(previewBuilding.gameObject);
    }

    public void MovePreview(GridPoint point)
    {
        previewBuilding.transform.parent = point.transform;
        previewBuilding.transform.localPosition = Vector3.up * previewBuilding.buildingOffset;
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