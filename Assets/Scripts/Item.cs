
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public GameObject itemDropPrefab;
    public bool isStackable;
    public bool isDestroyable;
    public bool isPainkiller;
    public int healthIncrease = 20;

    public virtual void Use()
    {
        // If the item is a painkiller
        if (isPainkiller)
        {
            // Get the player
            var player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Increase the player's health
                // Assumes the player has a FirstPersonController component
                var playerHealth = player.GetComponent<FirstPersonController>();
                if (playerHealth != null)
                {
                    playerHealth.IncreaseHealth(healthIncrease);
                    
                }
            }
            
        }
        else
        {
            // If the item is not a painkiller, do something else
        }
    }
}



