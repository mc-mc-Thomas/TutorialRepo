using UnityEngine;
using UnityEngine.UI;

public class ChestTriggerScript : MonoBehaviour
{
    bool playerInRange = false;
    bool chestOpen = false;
    private Transform parent;
    public GameObject chestUI;

    private string chestID;

    private string

    public InventoryManager inventoryManager;

    void Start()
    {
        chestID = Guid.NewGuid().ToString();
        parent = GameObject.Find("InventoryCanvas").transform;

        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager == null)
                Debug.LogWarning("InventoryManager nicht gefunden. Weisen Sie die Referenz im Inspector zu.");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!chestOpen)
                OpenChest();
            else
                CloseChest();
        }
    }

    void OpenChest()
    {
        chestOpen = true;
        Debug.Log(" opened!");

        inventoryManager.SetActive();

        Instantiate(chestUI, parent);
    }

    void CloseChest()
    {
        chestOpen = false;
        inventoryManager.SetunActive();

        StreamWriter sw = new StreamWriter("chest.txt");

        //sw.WriteLine("Hello World!!");

        //for(int i = 0; i < 16; i++)
        //{
        //    if ()
        //}


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CloseChest();
        }
            
        
    }
}
