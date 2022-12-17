using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Brewing/Ingredient List")]
public class IngredientList : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private bool alwaysReset;
    [SerializeField] public ObservableCollection<IngredientScriptableObject> value = new();

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if (!alwaysReset)
            return;

        for (var i = value.Count - 1; i >= 0; i--) value.RemoveAt(i);
    }
}