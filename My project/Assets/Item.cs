using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    public int ID;
    public int quantity = 1;

    public TextMeshProUGUI quantityText;


    private void Awake()
    {
        UpdateQuantityDisplay();
    }

    public void UpdateQuantityDisplay()
    {
        quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
            quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }


    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
            Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;   
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }



    public virtual void UseItem()
    {
        Debug.Log("Using item: " + ID);
    }
}