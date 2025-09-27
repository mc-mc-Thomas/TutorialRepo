using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;

    public virtual void UseItem()
    {
        Debug.Log("Using item: " + ID);
    }   
}
