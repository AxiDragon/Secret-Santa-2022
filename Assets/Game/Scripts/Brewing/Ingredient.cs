using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour
{
    public IngredientScriptableObject ingredientScriptableObject;
    private bool pickedUp;

    public IngredientScriptableObject PickUp(out bool success)
    {
        if (pickedUp)
        {
            success = false;
            return null;
        }

        pickedUp = true;
        transform.DOScale(0f, 1f).SetEase(Ease.InBounce).OnComplete(() => Destroy(gameObject));

        success = true;
        return ingredientScriptableObject;
    }
}