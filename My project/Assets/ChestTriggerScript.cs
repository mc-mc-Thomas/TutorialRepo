using UnityEngine;
using UnityEngine.UI;

public class ChestTriggerScript : MonoBehaviour
{
    bool playerInRange = false;
    bool chestOpen = false;
    private Transform parent;
    public GameObject chestUI;

    public GameObject InvMenu;

    void Start()
    {
        parent = GameObject.Find("InventoryCanvas").transform;
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

        InvMenu.SetActive(true);

        Instantiate(chestUI, parent);


    }

    void CloseChest()
    {
        chestOpen = false;
        Debug.Log(" closed!");
        // Hier kommt dein Close-Code hin (Animation, Sound etc.)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
