using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    public static InventoryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();

       
    }

    //public bool AddItem(GameObject itemPrefab)
    //{

    //    //neu f�r Item stacking 
    //    Item itemToAdd = itemPrefab.GetComponent<Item>();
    //    if (itemToAdd == null) return false;



    //    //Check if we have this item in inventory
    //    foreach (Transform slotTransform in inventoryPanel.transform)
    //    {
    //        Slot slot = slotTransform.GetComponent<Slot>();
    //        if (slot != null && slot.currentItem != null)
    //        {
    //            Item slotitem = slot.currentItem.GetComponent<Item>();
    //            if (slotitem != null && slotitem.ID == itemToAdd.ID)
    //            {
    //                //Same item stack them 
    //                slotitem.AddToStack();
    //                return true;
    //            }


    //        }
    //    }
    //    //Look for empty slot
    //    foreach (Transform slotTransform in inventoryPanel.transform)
    //    {
    //        Slot slot = slotTransform.GetComponent<Slot>();
    //        if (slot != null && slot.currentItem == null)
    //        {
    //            GameObject newItem = Instantiate(itemPrefab, slot.transform);
    //            newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    //            slot.currentItem = newItem;
    //            return true;


    //        }
    //    }
    //    Debug.Log("Inventory is full!");
    //    return false;
    //}






    public bool AddItem(GameObject itemPrefab)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null) return false;

        int amountToAdd = itemToAdd.quantity; // Menge im aufgehobenen Item

        // ------------------------------------------
        // 1. EXISTIERENDE STACKS F�LLEN
        // ------------------------------------------
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();

                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    // Wieviel Platz ist noch im Stack?
                    int freeSpace = Item.maxStack - slotItem.quantity;

                    if (freeSpace > 0)
                    {
                        int amountToPut = Mathf.Min(freeSpace, amountToAdd);

                        slotItem.AddToStack(amountToPut);
                        amountToAdd -= amountToPut;

                        if (amountToAdd <= 0)
                            return true;
                    }
                }
            }
        }

        // ------------------------------------------
        // 2. NEUE SLOTS F�LLEN (falls noch Menge �brig ist)
        // ------------------------------------------
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)
            {
                int newStackAmount = Mathf.Min(amountToAdd, Item.maxStack);

                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                Item newItemComp = newItem.GetComponent<Item>();
                newItemComp.quantity = newStackAmount;
                newItemComp.UpdateQuantityDisplay();

                slot.currentItem = newItem;

                amountToAdd -= newStackAmount;

                if (amountToAdd <= 0)
                    return true;
            }
        }

        Debug.Log("Inventory FULL!");
        return false;
    }








    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTranform.GetSiblingIndex(), quantity = item.quantity });
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        //Clear inventory panel - avoid duplicates
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        //Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        //Populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    Item itemComponent = item.GetComponent<Item>();
                    if (itemComponent != null && data.quantity > 1)
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDisplay();
                    }

                    slot.currentItem = item;
                }
            }
        }
    }
    public bool RemoveItems(int itemID, int amount)
    {
        int removed = 0;

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (removed >= amount)
                break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.ID == itemID)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                    removed++;
                }
            }
        }

        return removed == amount;
    }

}