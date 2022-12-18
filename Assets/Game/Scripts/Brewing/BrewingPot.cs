using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class BrewingPot : MonoBehaviour
{
    public RecipeScriptableObject[] recipes;

    public RecipeScriptableObject GetFittingRecipe(ObservableCollection<IngredientScriptableObject> inputIngredients)
    {
        var inputIngredientsInstance = inputIngredients.ToList();
        for (var i = 0; i < recipes.Length; i++)
        {
            var recipeIngredients = recipes[i].ingredients;
            if (recipeIngredients.OrderBy(x => x.ingredientName)
                .SequenceEqual(inputIngredientsInstance.OrderBy(x => x.ingredientName)))
                return recipes[i];
        }

        return null;
    }

    public Building Brew(ref ObservableCollection<IngredientScriptableObject> ingredients,
        RecipeScriptableObject recipe)
    {
        for (var i = 0; i < recipe.ingredients.Count; i++) ingredients.Remove(recipe.ingredients[i]);

        return recipe.resultBuilding;
    }
}