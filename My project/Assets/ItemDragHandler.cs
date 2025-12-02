using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 50f;

    public float maxDropDistance = 70f;

    Vector2 position2 = new Vector2(10f, 15f);


    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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
                //Slot has an item - swap items
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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
                if (!IsWithinInventory(eventData.position))
                {
                    // Prüfe, ob dieses Item ein Welt-Prefab platzieren soll
                    PlaceableUIItem placeable = GetComponent<PlaceableUIItem>();
                    if (placeable != null)
                    {
                        PlacePrefabOnGrid(placeable.worldPrefab);
                        Destroy(gameObject); // UI Item entfernen
                        return;
                    }

                    // Normales Drop-Item-Verhalten
                    DropItem(originalSlot);
                }

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
        originalSlot.currentItem = null;

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

        //Destroy the UI
        Destroy(gameObject);
    }
    void PlacePrefabOnGrid(GameObject prefab)
    {
        if (prefab == null)
            return;

        // Welt-Grid holen
        Grid grid = GetGridByName("Collision");

        if (grid == null)
        {
            Debug.LogError("Kein Grid in der Szene gefunden!");
            return;
        }

        // Mausposition in Weltkoordinaten
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Welt → Grid Cell
        Vector3Int cell = grid.WorldToCell(mouseWorld);

        // Mitte der Zelle holen (perfektes Snapping)
        Vector3 cellCenter = grid.GetCellCenterWorld(cell);

        // Prefab spawnen
        Instantiate(prefab, cellCenter, Quaternion.identity);
    }
    Grid GetGridByName(string name)
    {
        Grid[] grids = FindObjectsOfType<Grid>();

        foreach (Grid g in grids)
        {
            if (g.gameObject.name == name)
                return g;
        }

        Debug.LogError("Kein Grid mit dem Namen '" + name + "' gefunden!");
        return null;
    }




}