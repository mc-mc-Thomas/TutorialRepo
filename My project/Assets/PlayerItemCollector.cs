using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController InventoryController;



    // Start is called before the first frame update
    void Start()
    {
        InventoryController = FindObjectOfType<InventoryController>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            

            Item item = collision.GetComponent<Item>();
            
            if (item != null)
            {
                //add Item inventory
                bool itemAdded = InventoryController.AddItem(collision.gameObject);
                Destroy(collision.gameObject);
                Debug.Log("Mähhh");
            }
        }
    }

   
}
