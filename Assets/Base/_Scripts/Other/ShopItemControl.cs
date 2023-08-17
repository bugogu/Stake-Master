using UnityEngine;

public class ShopItemControl : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image contentItemImage;
    [SerializeField] private Color itemNotSelectedColor;
    [SerializeField] private Color itemSelectedColor;
    [SerializeField] UnityEngine.UI.Image characterImage;
    [SerializeField] TMPro.TMP_Text characterName;
    [SerializeField] TMPro.TMP_Text priceText;
    [SerializeField] UnityEngine.UI.Button characterPurchaseButton;
    [Space]
    [SerializeField] UnityEngine.UI.Button itemButton;

    public void SetItemPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition += pos;
    }
    public void SetCharacterImage(Sprite image)
    {
        characterImage.sprite = image;
    }
    public void SetCharacterName(string name)
    {
        characterName.text = name;
    }
    public void SetCharacterPrice(int price)
    {
        priceText.text = price.ToString();
    }
    public void SetCharacterAsPurchased()
    {
        contentItemImage.color = itemNotSelectedColor;
        characterPurchaseButton.gameObject.SetActive(false);
        itemButton.interactable = true;
    }
    public void OnItemPurchase(int itemIndex, UnityEngine.Events.UnityAction<int> action)
    {
        characterPurchaseButton.onClick.RemoveAllListeners();
        characterPurchaseButton.onClick.AddListener(() =>
        {
            action.Invoke(itemIndex);
        });
    }
    public void OnItemSelect(int itemIndex, UnityEngine.Events.UnityAction<int> action)
    {
        itemButton.interactable = true;
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() =>
        {
            action.Invoke(itemIndex);
        });
    }
    public void SelectItem()
    {
        contentItemImage.color = itemSelectedColor;
        itemButton.interactable = false;
    }
    public void DeselectItem()
    {
        contentItemImage.color = itemNotSelectedColor;
        itemButton.interactable = true;
    }
}
