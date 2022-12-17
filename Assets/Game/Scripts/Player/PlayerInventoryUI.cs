using System.Collections.Specialized;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private IngredientList ingredientList;
    private ItemUI[] uiItems;

    private void Awake()
    {
        uiItems = GetComponentsInChildren<ItemUI>();
        ingredientList.value.CollectionChanged += UpdateUI;
    }

    private void UpdateUI(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
        for (var i = 0; i < uiItems.Length; i++)
            if (i < ingredientList.value.Count)
            {
                var ingredientSprite = ingredientList.value[i].sprite;
                uiItems[i].SetSprite(ingredientSprite);
            }
            else
            {
                uiItems[i].ClearSprite();
            }
    }
}