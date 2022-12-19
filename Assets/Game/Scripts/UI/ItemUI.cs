using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] public Image itemSprite;

    public void ClearSprite()
    {
        itemSprite.transform.DOScale(0f, .5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            itemSprite.color = Vector4.zero;
            itemSprite.sprite = null;
        });
    }

    public void SetSprite(Sprite sprite)
    {
        itemSprite.color = Vector4.one;
        itemSprite.sprite = sprite;

        itemSprite.transform.localScale = Vector3.zero;
        itemSprite.transform.DOScale(1f, .5f).SetEase(Ease.OutBack);
    }
}