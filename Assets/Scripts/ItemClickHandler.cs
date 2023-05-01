using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{
   public void OnItemClicked()
   {
      ItemDragHandler dragHandler = gameObject.transform.Find("itemImage").GetComponent<ItemDragHandler>();

      IInventoryItem item = dragHandler.Item;
      
      Debug.Log(item.Name);
      
      
   }
}
