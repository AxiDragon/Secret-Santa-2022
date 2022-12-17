using UnityEngine;

[CreateAssetMenu(menuName = "Brewing/Ingredient", fileName = "Ingredient")]
public class IngredientScriptableObject : ScriptableObject
{
    public string ingredientName;
    public Sprite sprite;
}