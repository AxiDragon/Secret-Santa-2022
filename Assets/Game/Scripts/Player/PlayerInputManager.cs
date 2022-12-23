using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private BuildingConstructor buildingConstructor;
    private PlayerBrewer playerBrewer;
    private PlayerInventory playerInventory;
    private PauseMenu pauseMenu;

    private void Awake()
    {
        buildingConstructor = GetComponent<BuildingConstructor>();
        playerBrewer = GetComponent<PlayerBrewer>();
        playerInventory = GetComponent<PlayerInventory>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void InputTab(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;
        
        buildingConstructor.ToggleBuildMode();
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
        
        if (buildingConstructor.CurrentBuilding)
            return;
        
        playerBrewer.AttemptBrew(out var success);

        if (success)
            return;

        playerInventory.GatherIngredients();
    }

    public void InputQ(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;
        
        if (buildingConstructor.buildModeActive)
            buildingConstructor.RotateBuilding();
    }

    public void InputEscape(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;
        
        pauseMenu.DisplayPauseMenu();
    }
}