using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;   // <-- Wichtig für Tilemap

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 50f;
    public float maxDropDistance = 70f;

    public GameObject chestTriggerPrefab;

    private InventoryController inventoryController;


    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController =  InventoryController.Instance;    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    canvasGroup.blocksRaycasts = true; //Enables raycasts
    //    canvasGroup.alpha = 1f; //No longer transparent

    //    Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //Slot where item dropped
    //    if (dropSlot == null)
    //    {
    //        GameObject dropItem = eventData.pointerEnter;
    //        if (dropItem != null)
    //        {
    //            dropSlot = dropItem.GetComponentInParent<Slot>();
    //        }
    //    }
    //    Slot originalSlot = originalParent.GetComponent<Slot>();

    //    if (dropSlot != null)
    //    {

    //        //Is a slot under drop point
    //        if (dropSlot.currentItem != null)
    //        {
    //            Item draggedItem = GetComponent<Item>();
    //            Item targetItem = dropSlot.currentItem.GetComponent<Item>();

    //            if (draggedItem.ID == targetItem.ID)
    //            {
    //                targetItem.AddToStack(draggedItem.quantity);
    //                originalSlot.currentItem = null;
    //                Destroy(gameObject);
    //            }
    //            else
    //            {

    //                //Slot has an item - swap items
    //                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
    //                originalSlot.currentItem = dropSlot.currentItem;
    //                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;


    //                transform.SetParent(dropSlot.transform);
    //                dropSlot.currentItem = gameObject;
    //                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center

    //            }




    //        }
    //        else
    //        {
    //            originalSlot.currentItem = null;
    //            transform.SetParent(dropSlot.transform);
    //            dropSlot.currentItem = gameObject;
    //            GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
    //        }


    //    }
    //    else
    //    {
    //        //If we are dropping is not within the inventory
    //        if (!IsWithinInventory(eventData.position))
    //        {
    //            //Drop our item
    //            DropItem(originalSlot);
    //        }
    //        else
    //        {
    //            transform.SetParent(originalParent);
    //            GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
    //        }

    //        //else
    //        //No slot under drop point
    //        transform.SetParent(originalParent);
    //    }


    //}


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Enabling raycasts
        canvasGroup.alpha = 1f; // Reset transparency

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
                dropSlot = dropItem.GetComponentInParent<Slot>();
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();
        Item draggedItem = GetComponent<Item>();

        // --- Wenn in ein Inventar-Slot gedroppt ---
        if (dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if (draggedItem.ID == targetItem.ID)
                {
                    // ---- STACK LOGIK ----
                    int freeSpace = Item.maxStack - targetItem.quantity;

                    if (freeSpace > 0)
                    {
                        int amountToAdd = Mathf.Min(freeSpace, draggedItem.quantity);
                        targetItem.AddToStack(amountToAdd);

                        draggedItem.quantity -= amountToAdd;
                        draggedItem.UpdateQuantityDisplay();

                        if (draggedItem.quantity <= 0)
                        {
                            originalSlot.currentItem = null;
                            Destroy(gameObject);
                        }
                        else
                        {
                            // Rest zurück in Originalslot
                            transform.SetParent(originalSlot.transform);
                            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }
                    }
                    else
                    {
                        // Kein Platz → zurück in Originalslot
                        transform.SetParent(originalSlot.transform);
                        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    }
                }
                else
                {
                    // ---- ITEM SWAP ----
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                // ---- LEERER SLOT ----
                originalSlot.currentItem = null;
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;

            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return;
        }

        if (!IsWithinInventory(eventData.position))
        {
            PlaceableUIItem placeable = GetComponent<PlaceableUIItem>();
            if (placeable != null)
            {
                PlaceTileOnTilemap(placeable.tileSprite);
                Destroy(gameObject);  // UI Item entfernen
                return;
            }
            DropItem(originalSlot);
        }
        else
        {
            // ---- AUS INVENTAR DROPPEN ----
            if (!IsWithinInventory(eventData.position))
            {
                DropItem(originalSlot);
            }
            else
            {
                transform.SetParent(originalSlot.transform);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
    }

    bool IsWithinInventory(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        int quantity = item.quantity;   

        if (quantity > 1)
        {
            item.RemoveFromStack();

            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            quantity = 1;   
        }
        else
        {
            originalSlot.currentItem = null;

        }



       

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag");
            return;
        }

        Vector3 mausWeltPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mausWeltPosition.z = 0f;

        Instantiate(gameObject, mausWeltPosition, Quaternion.identity);
        //Item droppedItem = dropItem.GetComponent<Item>();
        //droppedItem.quantity = 1;

        //Destroy the UI
        if (quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);

        } 
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }

    }

    public void SplitStack()
    {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1) return;

        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount);

        GameObject newItem = item.CloneItem(splitAmount);

        if (inventoryController == null || newItem == null) return;

        foreach(Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;

            }
        }


        //No empty stack

        item.AddToStack(splitAmount);
        Destroy(newItem);

        InventoryController inventory = FindObjectOfType<InventoryController>(); //muss vllt weg
    }


    void PlaceTileOnTilemap(Sprite sprite)
    {
        if (sprite == null)
            return;

        Tilemap tilemap = GetTilemapByName("Collision");

        if (tilemap == null)
        {
            Debug.LogError("Tilemap 'Collision' nicht gefunden!");
            return;
        }

        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMouse.z = 0f;

        Vector3Int cell = tilemap.WorldToCell(worldMouse);

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;

        tilemap.SetTile(cell, tile);

        // ChestTrigger an Position spawnen
        Vector3 cellCenter = tilemap.GetCellCenterWorld(cell);
        Instantiate(chestTriggerPrefab, cellCenter, Quaternion.identity);
    }


    Tilemap GetTilemapByName(string name)
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (Tilemap m in maps)
        {
            if (m.gameObject.name == name)
            {
                Debug.Log("Tilemap gefunden: " + name);
                return m;
            }
        }

        Debug.LogError("Keine Tilemap mit dem Namen '" + name + "' gefunden!");
        return null;
    }
}
