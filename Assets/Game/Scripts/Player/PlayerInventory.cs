using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public IngredientList ingredients;
    [SerializeField] private float pickupCheckRange = 5f;
    [SerializeField] private LayerMask pickupLayerMask;
    public bool canPickup;
    private PlayerInventoryUI playerInventoryUI;
    private BuildingConstructor buildingConstructor;

    public bool CanPickup
    {
        get => canPickup;
        set
        {
            bool previousValue = canPickup;
            canPickup = value;

            if (previousValue != value)
            {
                playerInventoryUI.DisplayTooltip(canPickup, "e", "gather");
            }
        }
    }

    private void Awake()
    {
        playerInventoryUI = FindObjectOfType<PlayerInventoryUI>();
        buildingConstructor = GetComponent<BuildingConstructor>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupCheckRange);
    }

    private void Update()
    {
        CanPickup = CheckCanPickup();
    }

    private bool CheckCanPickup()
    {
        if (buildingConstructor.CurrentBuilding != null)
            return false;
        
        if (ingredients.value.Count >= 3)
            return false;

        return Physics.CheckSphere(transform.position, pickupCheckRange, pickupLayerMask);
    }

    public void GatherIngredients()
    {
        var inRangeIngredients = new List<IngredientInfo>();

        foreach (var c in Physics.OverlapSphere(transform.position, pickupCheckRange, pickupLayerMask))
        {
            if (c.TryGetComponent(out Ingredient ingredient))
            {
                var ingredientInfo = new IngredientInfo(ingredient,
                    Vector3.Distance(transform.position, ingredient.transform.position));

                for (var i = 0; i <= inRangeIngredients.Count; i++)
                {
                    if (i == inRangeIngredients.Count)
                    {
                        inRangeIngredients.Insert(i, ingredientInfo);
                        break;
                    }

                    if (ingredientInfo.distanceToIngredient < inRangeIngredients[i].distanceToIngredient)
                    {
                        inRangeIngredients.Insert(i, ingredientInfo);
                        break;
                    }
                }
            }
        }

        for (var i = 0; i < inRangeIngredients.Count; i++)
        {
            if (ingredients.value.Count >= 3)
                break;

            var ingredientPickup =
                inRangeIngredients[i].ingredient.PickUp(out var success);

            if (success)
            {
                ingredients.value.Add(ingredientPickup);
                break;
            }
        }
    }

    private struct IngredientInfo
    {
        public readonly Ingredient ingredient;
        public readonly float distanceToIngredient;

        public IngredientInfo(Ingredient ingredient, float distanceToIngredient)
        {
            this.ingredient = ingredient;
            this.distanceToIngredient = distanceToIngredient;
        }
    }
}