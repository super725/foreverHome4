using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : Interaction
{
    
    public override void OnFocus()
    {
        //highlight method
    }
    public override void OnInteract()
    {
        print("Interacted with " + gameObject.name);
    }
    public override void OnLoseFocus()
    {
        print("Stopped looking at " + gameObject.name);
    }
}
