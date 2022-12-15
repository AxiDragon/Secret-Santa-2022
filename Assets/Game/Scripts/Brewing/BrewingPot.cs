using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrewingPot : MonoBehaviour
{
    public RecipeScriptableObject[] recipes;

    public RecipeScriptableObject GetFittingRecipe(List<IngredientScriptableObject> inputIngredients)
    {
        var inputIngredientsInstance = new List<IngredientScriptableObject>(inputIngredients);
        for (int i = 0; i < recipes.Length; i++)
        {
            var recipeIngredients = recipes[i].ingredients;
            if (recipeIngredients.OrderBy(x => x)
                .SequenceEqual(inputIngredientsInstance.OrderBy(x => x)))
            {
                return recipes[i];
            }
        }

        return null;
    }
    
    public Building Brew(ref List<IngredientScriptableObject> ingredients, RecipeScriptableObject recipe)
    {
        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            ingredients.Remove(recipe.ingredients[i]);
        }

        return recipe.resultBuilding;
    }
}
