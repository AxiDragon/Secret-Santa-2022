using System;
using FMODUnity;
using UnityEngine;

public class PlayerBrewer : MonoBehaviour
{
    [SerializeField] private float brewingRange = 2.5f;
    [SerializeField] private LayerMask brewingPotMask;
    [SerializeField] private IngredientList ingredientList;
    [SerializeField] private EventReference brewSoundEffectReference;
    private bool canBrew;
    private PlayerInventoryUI playerInventoryUI;
    private BuildingConstructor buildingConstructor;

    public bool CanBrew
    {
        get => canBrew;
        set
        {
            bool previousValue = canBrew;
            canBrew = value;

            if (previousValue != value)
            {
                playerInventoryUI.DisplayTooltip(canBrew, "e", "brew");
            }
        }
    }

    private void Awake()
    {
        playerInventoryUI = FindObjectOfType<PlayerInventoryUI>();
        buildingConstructor = GetComponent<BuildingConstructor>();
    }

    private void Update()
    {
        CanBrew = CheckCanBrew();
    }

    private bool CheckCanBrew()
    {
        if (ingredientList.value.Count < 3)
            return false;

        RecipeScriptableObject recipe = null;
        foreach (var brewingPotCollider in Physics.OverlapSphere(transform.position, brewingRange, brewingPotMask))
            if (brewingPotCollider.TryGetComponent(out BrewingPot brewingPot))
            {
                recipe = brewingPot.GetFittingRecipe(ingredientList.value);
            }

        return recipe != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, brewingRange);
    }

    
    public void AttemptBrew(out bool success)
    {
        success = false;

        if (ingredientList.value.Count < 3)
            return;

        foreach (var brewingPotCollider in Physics.OverlapSphere(transform.position, brewingRange, brewingPotMask))
            if (brewingPotCollider.TryGetComponent(out BrewingPot brewingPot))
            {
                var recipe = brewingPot.GetFittingRecipe(ingredientList.value);

                if (recipe != null)
                {
                    buildingConstructor.CurrentBuilding = brewingPot.Brew(ref ingredientList.value, recipe);
                    RuntimeManager.PlayOneShot(brewSoundEffectReference);
                    success = true;
                }
            }
    }
}