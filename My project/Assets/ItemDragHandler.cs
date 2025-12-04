using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 50f;

    public float maxDropDistance = 70f;

    Vector2 position2 = new Vector2(10f, 15f);

    private InventoryController inventoryController;


    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController =  InventoryController.Instance;    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //Save OG parent
        transform.SetParent(transform.root); //Above other canvas'
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; //Semi-transparent during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; //Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; //Enables raycasts
        canvasGroup.alpha = 1f; //No longer transparent

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //Slot where item dropped
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {

            //Is a slot under drop point
            if (dropSlot.currentItem != null)
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if (draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {

                    //Slot has an item - swap items
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                }



               
            }
            else
            {
                originalSlot.currentItem = null;
            }

            //Move item into drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            //If we are dropping is not within the inventory
            if (!IsWithinInventory(eventData.position))
            {
                //Drop our item
                DropItem(originalSlot);
            }
            else
            {
                transform.SetParent(originalParent);
            }

            //else
            //No slot under drop point
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
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

            tranform.SetParent(originalParent);
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
        //Random drop position
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;

        Vector3 mausBildschirmPosition = Input.mousePosition;

        // 2️⃣ In Weltkoordinaten umwandeln
        Vector3 mausWeltPosition = Camera.main.ScreenToWorldPoint(mausBildschirmPosition);

        // 3️⃣ Z auf 0 setzen (damit es im 2D-Raum liegt)
        mausWeltPosition.z = 0f;

        //Instatiate drop Item
        Instantiate(gameObject, mausWeltPosition, Quaternion.identity);
        Item droppedItem = dropItem.GetComponent<Item>();
        droppedItem.quantity = 1;

        //Destroy the UI
        if (quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);

        } 
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.ImputButton.Right)
        {
            //split stack
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

        }


        InventoryController inventory = FindObjectOfType<InventoryController>(); //muss vllt weg
    }




}