
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    InventoryItem inventoryItem;
    public TMP_Text quantity;

    public void AddItem(InventoryItem newInventoryItem)
    {
        inventoryItem = newInventoryItem;

        icon.sprite = inventoryItem.item.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        if (inventoryItem.item.isStackable)
        {
            quantity.text = inventoryItem.quantity.ToString();
            quantity.enabled = true;
        }
        else
        {
            quantity.enabled = false; 
        }
    }

    public void ClearSlot()
    {
        inventoryItem = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        quantity.enabled = false;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(inventoryItem);
    }

    public void UseItem()
    {
        if (inventoryItem != null)
        {
            inventoryItem.item.Use();
            if (inventoryItem.item.isStackable)
            {
                 inventoryItem.quantity--;
                
                    if (inventoryItem.quantity == 0)
                    {
                        
                        ClearSlot();
                        Inventory.instance.Remove(inventoryItem);
                    }
                    else
                    {
                        quantity.text = inventoryItem.quantity.ToString();
                    }
            }
           
        }
    }

    
    
}
