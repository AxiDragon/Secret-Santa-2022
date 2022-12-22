using System.Collections.Specialized;
using DG.Tweening;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private IngredientList ingredientList;
    [SerializeField] private RectTransform itemBlocker;
    public Tooltip[] tooltips;
    [SerializeField] private float tooltipSwitchingTime = .25f;
    private ItemUI[] uiItems;

    private void Awake()
    {
        tooltips = GetComponentsInChildren<Tooltip>();
        uiItems = GetComponentsInChildren<ItemUI>();
        ingredientList.value.CollectionChanged += UpdateItemUI;
    }

    private void UpdateItemUI(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
        for (var i = 0; i < uiItems.Length; i++)
            if (i < ingredientList.value.Count)
            {
                Sprite ingredientSprite = ingredientList.value[i].sprite;

                if (ingredientSprite != uiItems[i].itemSprite.sprite)
                    uiItems[i].SetSprite(ingredientSprite);
            }
            else
            {
                uiItems[i].ClearSprite();
            }
    }

    public void DisplayInventory(bool show)
    {
        float targetPosition = show ? 0f : -200f;
        itemBlocker.DOMoveY(targetPosition, 1f).SetEase(Ease.InOutSine);
    }

    public void DisplayTooltip(bool show, string button, string functionPrompt, int id = 0)
    {
        if (tooltips[id].tooltipDisplayed && show)
        {
            SwapTooltip(button, functionPrompt);
            return;
        }

        tooltips[id].tooltipDisplayed = true;
        
        float targetPosition = show ? 100f * id : -100f;
        tooltips[id].transform.DOMoveY(targetPosition, tooltipSwitchingTime).SetEase(Ease.InOutSine);
        tooltips[id].buttonPrompt.text = button;
        tooltips[id].functionPrompt.text = functionPrompt;
    }

    public void DisplayTooltip(bool show, int id = 0)
    {
        tooltips[id].tooltipDisplayed = show;
        float targetPosition = show ? 100f * id : -100f;
        tooltips[id].transform.DOMoveY(targetPosition, tooltipSwitchingTime).SetEase(Ease.InOutSine);
    }

    private void SwapTooltip(string button, string functionPrompt, int id = 0)
    {
        tooltips[id].transform.DOMoveY(-100f, tooltipSwitchingTime).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            tooltips[id].buttonPrompt.text = button;
            tooltips[id].functionPrompt.text = functionPrompt;
            tooltips[id].transform.DOMoveY(100f * id, tooltipSwitchingTime).SetEase(Ease.InOutSine);
        });
    }
}