using UnityEngine;
using UnityEngine.UI;

public class ChestTriggerScript : MonoBehaviour
{
    bool playerInRange = false;
    bool chestOpen = false;
    private Transform parent;
    public GameObject chestUI;

    private GameObject chestUIInstance;
    private InventoryManager inventoryManager;

    private bool invStartPoint;

    void Start()
    {
        parent = GameObject.Find("InventoryCanvas").transform;

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        
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
        invStartPoint = inventoryManager.menuActivated;

        if (!invStartPoint)
            inventoryManager.SetActive();

        chestOpen = true;

        
        if (chestUIInstance == null)
        {
            chestUIInstance = Instantiate(chestUI, parent);
            chestUIInstance.SetActive(true);
        }
        else
        {
            chestUIInstance.SetActive(true);
        }
        

    }

    void CloseChest()
    {
        chestOpen = false;
        chestUIInstance.SetActive(false);

        if (!invStartPoint)
            inventoryManager.SetunActive();
        else
            inventoryManager.SetActive();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInRange = false;
        CloseChest(); 
    }
}
