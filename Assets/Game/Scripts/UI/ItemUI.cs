using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;

    public void ClearSprite()
    {
        itemSprite.color = Vector4.zero;
        itemSprite.sprite = null;
    }

    public void SetSprite(Sprite sprite)
    {
        itemSprite.color = Vector4.one;
        itemSprite.sprite = sprite;
    }
}