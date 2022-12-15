using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Brewing/Ingredient", fileName = "Ingredient")]
public class IngredientScriptableObject : ScriptableObject
{
    public string ingredientName;
    public Texture texture;
}
