using UnityEngine;
using UnityEngine.UI;

public class craftingTriggerScript : MonoBehaviour
{
    bool playerInRange = false;
    bool crafterOpen = false;
    private Transform parent;
    public GameObject crafterUI;

    private GameObject crafterUIInstance;
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
            if (!crafterOpen)
                OpenCrafter();
            else
                CloseCrafter();
        }
    }

    void OpenCrafter()
    {
        invStartPoint = inventoryManager.menuActivated;

        if (!invStartPoint)
            inventoryManager.SetActive();

        crafterOpen = true;


        if (crafterUIInstance == null)
        {
            crafterUIInstance = Instantiate(crafterUI, parent);
            crafterUIInstance.SetActive(true);
        }
        else
        {
            crafterUIInstance.SetActive(true);
        }


    }

    void CloseCrafter()
    {
        crafterOpen = false;
        crafterUIInstance.SetActive(false);

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
        CloseCrafter();
    }
}
