using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class hotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotcount = 6;

    //private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    private void Awake()
    {
        //itemDictionary = FindObjectOfType<itemDictionary>();

        hotbarKeys = new Key[slotcount];
        for (int i = 0; i < slotcount; i++)
        {
            hotbarKeys[i] = i < 9  ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < slotcount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                //UseItemInSlot(i);
            }
        }
    }
    void UseItemInSlot(int index)
    {
        //Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        //if (slot != null)
        //{
        //    Item item = slot.currentItem.GetComponent<Item>();
        //    item.Useitem();
        //}
    }
}
