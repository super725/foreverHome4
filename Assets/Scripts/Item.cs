
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);

    public virtual void Use()
    {
        //Use item
        //Something happens
        
        Debug.Log("Using " + name );
    }
}

public enum ItemType
{
    BuildingBlock,
    Tool
}

public enum ActionType
{
    Attack, 
    
}

