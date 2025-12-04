using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;   // <-- Wichtig für Tilemap

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 50f;
    public float maxDropDistance = 70f;

    public GameObject chestTriggerPrefab;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
                dropSlot = dropItem.GetComponentInParent<Slot>();
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        // --- Wenn in ein Inventar-Slot gedroppt ---
        if (dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
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


            // normales Item droppen
            DropItem(originalSlot);
        }
        else
        {
            transform.SetParent(originalParent);
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
        originalSlot.currentItem = null;

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag");
            return;
        }

        Vector3 mausWeltPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mausWeltPosition.z = 0f;

        Instantiate(gameObject, mausWeltPosition, Quaternion.identity);
        Destroy(gameObject);
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
