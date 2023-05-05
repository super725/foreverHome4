
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image icon;
    public Button removeButton;
    
    Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag = transform;
        
        RectTransform droppedRectTransform = dropped.GetComponent<RectTransform>();
        droppedRectTransform.localPosition = Vector3.zero;
        droppedRectTransform.anchorMin = Vector2.zero;
        droppedRectTransform.anchorMax = Vector2.one;
        droppedRectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Force the Layout Group to update
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }
    
}
