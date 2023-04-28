using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
   public void Awake()
   {
      gameObject.layer = 6;
   }

   public abstract void OnInteract();
   public abstract void OnFocus();
   public abstract void OnLoseFocus();
}
