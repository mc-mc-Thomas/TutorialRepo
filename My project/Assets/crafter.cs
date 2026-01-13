using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class crafter : MonoBehaviour
{
    private InventoryController inventoryController;
    List<InventorySaveData> invData = new List<InventorySaveData>();
    public Slot slot1;

    static int craftItem = 0;
    private ItemDictionary itemDictionary;

    void Awake()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        itemDictionary = FindObjectOfType<ItemDictionary>();
    }


    public void chestcraft()
    {
        invData = inventoryController.GetInventoryItems();

        if (CountItem(3) >= 5)
        {
            craftItem = 10;
        }
    }
    public void craft()
    {
        if (craftItem == 10)
        {
            inventoryController.RemoveItems(3, 5);

            GameObject newItemPrefab = itemDictionary.GetItemPrefab(5);

            slot1.currentItem = Instantiate(newItemPrefab, slot1.transform);
            slot1.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            craftItem = 0;

        }
    }
    int CountItem(int itemId)
    {
        return invData.Count(i => i.itemID == itemId);
    }

}

