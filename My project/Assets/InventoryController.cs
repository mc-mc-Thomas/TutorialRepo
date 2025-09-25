using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotcount;
    public GameObject[] itemPrefabs;



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < slotcount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; 
                slot.currentItem = item;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
