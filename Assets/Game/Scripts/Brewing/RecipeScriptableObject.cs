using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Brewing/Recipe", fileName = "Recipe")]
public class RecipeScriptableObject : ScriptableObject
{
    public List<IngredientScriptableObject> ingredients;
    public Building resultBuilding;
    public Texture resultBuildingTexture;

    private void OnValidate()
    {
        if (ingredients.Count > 3)
        {
            Debug.LogWarning("Max of 3 Ingredients");
            for (var i = ingredients.Count - 1; i >= 3; i--) ingredients.RemoveAt(i);
        }
    }
}