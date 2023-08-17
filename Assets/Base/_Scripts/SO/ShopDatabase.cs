using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Shopping/Add New Shop Item")]
public class ShopDatabase : ScriptableObject
{
    public Item[] items;
    public int CharacterCount
    {
        get { return items.Length; }
    }
    public Item GetCharacter(int index)
    {
        return items[index];
    }
    public void Purchase(int index)
    {
        items[index].isPurchased = true;
    }
}
