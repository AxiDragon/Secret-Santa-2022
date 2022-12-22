using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour
{
    public IngredientScriptableObject ingredientScriptableObject;
    [SerializeField] private bool isInfinite;
    private float startingScale;
    private bool pickedUp;

    private void Awake()
    {
        startingScale = transform.localScale.x;
    }

    public IngredientScriptableObject PickUp(out bool success)
    {
        if (!isInfinite)
        {
            if (pickedUp)
            {
                success = false;
                return null;
            }
            pickedUp = true;
            transform.DOScale(0f, 1f).SetEase(Ease.InBounce).OnComplete(() => Destroy(gameObject));
        }
        else
        {
            transform.DOScale(startingScale * .85f, .2f).SetEase(Ease.InOutCirc)
                .OnComplete(() => transform.DOScale(startingScale, .2f).SetEase(Ease.InOutCirc));
        }

        success = true;
        
        return ingredientScriptableObject;
    }
}