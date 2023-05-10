using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public Item item;
    public int quantity;

    public InventoryItem(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class Inventory : MonoBehaviour
{
    #region Singleton
    
    
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of inventory Found!");
        }
        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();

    public OnItemChanged onItemChangedCallback;
    
    public int space = 20;
    [SerializeField] private bool createItemDropOnRemove = false;
    [SerializeField] private GameObject itemPrefab = null;
    
    public List<InventoryItem> items = new List<InventoryItem>();

    public bool Add(Item item)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space && !item.isStackable)
            {
                Debug.Log("Not enough Room");
                return false;
            }

            InventoryItem existingItem = items.Find(i => i.item == item);
            if (existingItem != null)
            {
                // Increase quantity if the item is stackable and already in the inventory
                existingItem.quantity++;
            }
            else
            {
                // Add new item to the inventory
                items.Add(new InventoryItem(item, 1));
            }

            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }

        return true;
    }

    public void Remove(InventoryItem inventoryItem)
    {
        if (items.Contains(inventoryItem))
        {
            inventoryItem.quantity--;

            if (inventoryItem.quantity == 0)
            {
                items.Remove(inventoryItem);
            }
            

            if (createItemDropOnRemove && inventoryItem.item.itemDropPrefab != null && !inventoryItem.item.isPainkiller)
            {
                // Create a new instance of the item at the player's position
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Instantiate(inventoryItem.item.itemDropPrefab, player.transform.position + player.transform.forward, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("Player gameobject not found. Item was not dropped.");
                }
            }
        
            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("Item not found in the inventory.");
            
        }
    }
}
