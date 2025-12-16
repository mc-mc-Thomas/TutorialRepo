using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InvMenu;
    internal bool menuActivated; //alle scripts können darauf zugreifen


    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && menuActivated)
        {
            InvMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.I) && !menuActivated)
        {
            InvMenu.SetActive(true);
            menuActivated = true;
        }
    }

    public void SetActive()
    {
        InvMenu.SetActive(true);
        menuActivated = true;
    }
    public void SetunActive()
    {
        InvMenu.SetActive(false);
        menuActivated = false;
    }

}
