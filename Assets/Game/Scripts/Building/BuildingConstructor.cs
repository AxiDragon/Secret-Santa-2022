using System.Linq;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class BuildingConstructor : MonoBehaviour
{
    [SerializeField] private Material previewMaterial;
    [SerializeField] private GridPointListVariable gridPointList;
    [SerializeField] private EventReference buildSoundEffectReference;
    private Building currentBuilding;
    private BuildingCollectionTracker buildingCollectionTracker;
    private VisualEffect buildEffect;

    public Building CurrentBuilding
    {
        get => currentBuilding;
        set
        {
            currentBuilding = value;
            playerInventoryUI.DisplayInventory(currentBuilding == null);
            if (currentBuilding != null)
                playerInventoryUI.DisplayTooltip(true, "tab", "build mode");
        }
    }

    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private UnityEvent activateBuildMode;
    [SerializeField] private UnityEvent deActivateBuildMode;
    [SerializeField] private float maxBuildDistance = 25f;
    [HideInInspector] public bool buildModeActive;
    private PlayerInventoryUI playerInventoryUI;
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
                playerInventoryUI.DisplayTooltip(true, "e", "build");
                playerInventoryUI.DisplayTooltip(true, "q", "rotate", 1);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                ClosestGridPoint = gridPointList.GetClosestGridPoint(GetMousePosition());
                GeneratePreview(CurrentBuilding);
            }
            else
            {
                deActivateBuildMode?.Invoke();
                Cursor.lockState = CursorLockMode.Locked;

                if (previewBuilding != null)
                    Destroy(previewBuilding.gameObject);
                
                playerInventoryUI.DisplayTooltip(false, 1);

                if (CurrentBuilding == null)
                {
                    playerInventoryUI.DisplayTooltip(false);
                }
                else
                    playerInventoryUI.DisplayTooltip(true, "tab", "build mode");
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

    private void Awake()
    {
        playerInventoryUI = FindObjectOfType<PlayerInventoryUI>();
        buildEffect = GetComponentInChildren<VisualEffect>();
        buildingCollectionTracker = FindObjectOfType<BuildingCollectionTracker>();
        buildEffect.transform.parent = null;
    }

    private void Update()
    {
        if (buildModeActive)
            ClosestGridPoint = gridPointList.GetClosestGridPoint(GetMousePosition());
    }

    private Vector3 GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, maxBuildDistance, raycastMask.value)) return hit.point;

        return transform.position;
    }

    public void Build()
    {
        var buildingInstance = Instantiate(CurrentBuilding, ClosestGridPoint.transform);

        var buildingInstanceTransform = buildingInstance.transform;
        buildingInstanceTransform.rotation = previewBuilding.transform.rotation;
        buildingInstanceTransform.localPosition = Vector3.zero;

        buildingInstance.AddPointsToGrid();

        buildEffect.transform.position = ClosestGridPoint.transform.position;
        buildEffect.Play();
        
        RuntimeManager.PlayOneShot(buildSoundEffectReference);

        if (buildingCollectionTracker != null)
            buildingCollectionTracker.AttemptAddBuildingToCollection(CurrentBuilding);
        
        ResetBuildMode();

        SpawnObjectEasing(buildingInstance.gameObject);
    }

    public void ToggleBuildMode()
    {
        if (CurrentBuilding)
            BuildModeActive = !BuildModeActive;
    }

    private void ResetBuildMode()
    {
        CurrentBuilding = null;
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
        previewBuilding.transform.localPosition = Vector3.zero;
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

    public void RotateBuilding()
    {
        if (previewBuilding != null)
        {
            previewBuilding.transform.eulerAngles += Vector3.up * 90f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxBuildDistance);
    }
}