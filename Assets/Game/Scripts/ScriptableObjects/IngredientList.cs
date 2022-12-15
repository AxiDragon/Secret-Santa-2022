using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Brewing/Ingredient List")]
public class IngredientList : ScriptableObject, ISerializationCallbackReceiver
{
    public List<IngredientScriptableObject> value;
    [SerializeField] bool alwaysReset;

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if (!alwaysReset)
            return;
        
        for (int i = value.Count - 1; i >= 0; i--)
        {
            value.RemoveAt(i);
        } 
    }
}
