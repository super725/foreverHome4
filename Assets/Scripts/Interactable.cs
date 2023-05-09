
using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
   public float radius = 3f;
   public Transform interactionTransform;

   bool isFocus = false;
   Transform player;

   bool hasInteracted = false;

   public virtual void Interact()
   {
      //to be overriden
      Debug.Log("Interacting with " + transform.name);
   }

   private void Update()
   {
      if (isFocus && !hasInteracted)
      {
         float distance = Vector3.Distance(player.position, interactionTransform.position);
         if (distance <= radius)
         {
            Debug.Log("Interact");
            Interact();
            hasInteracted = true;
         }
      }
   }

   public void OnFocused(Transform playerTransform)
   {
      isFocus = true;
      player = playerTransform;
      hasInteracted = false;
   }

   public void onDefocused()
   {
      isFocus = false;
      player = null;
      hasInteracted = false;
   }

   private void OnDrawGizmosSelected()
   {
      if (interactionTransform == null)
         interactionTransform = transform;
      
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(interactionTransform.position, radius);
   }
}

