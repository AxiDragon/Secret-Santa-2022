using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private BuildingConstructor buildingConstructor;
    private PlayerBrewer playerBrewer;
    private PlayerInventory playerInventory;

    private void Awake()
    {
        buildingConstructor = GetComponent<BuildingConstructor>();
        playerBrewer = GetComponent<PlayerBrewer>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    public void InputE(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;

        if (buildingConstructor.buildModeActive)
        {
            buildingConstructor.Build();
            return;
        }

        playerBrewer.AttemptBrew(out var success);

        if (success)
            return;

        playerInventory.GatherIngredients();
    }
}