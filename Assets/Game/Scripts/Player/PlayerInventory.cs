using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public IngredientList ingredients;
    [SerializeField] private float pickupCheckRange = 5f;
    [SerializeField] private LayerMask pickupLayerMask;

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

    public void GatherIngredientsInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            GatherIngredients();
        }
    }

    public void GatherIngredients()
    {
        List<IngredientInfo> inRangeIngredients = new List<IngredientInfo>();

        foreach (Collider c in Physics.OverlapSphere(transform.position, pickupCheckRange, pickupLayerMask))
        {
            if (c.TryGetComponent(out Ingredient ingredient))
            {
                print(ingredient.name);
                var ingredientInfo = new IngredientInfo(ingredient,
                    Vector3.Distance(transform.position, ingredient.transform.position));

                for (int i = 0; i <= inRangeIngredients.Count; i++)
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

        for (int i = 0; i < inRangeIngredients.Count; i++)
        {
            if (ingredients.value.Count >= 3)
                break;

            IngredientScriptableObject ingredientPickup =
                inRangeIngredients[i].ingredient.PickUp(out bool success);

            if (success)
                ingredients.value.Add(ingredientPickup);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupCheckRange);
    }
}