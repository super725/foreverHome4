using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public static event Action<string> OnInteract;
    private Camera cam;

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject interactableComponent;
    
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 p1 = cam.transform.position;
        
        // Physics.SphereCast(p1, 0.2f, ray.direction, out hit, 2f)
        Ray ray = cam.ScreenPointToRay(new (Screen.width/2f, Screen.height/2f, 0f));
        if (Physics.Raycast(ray, out hit,1f))
        {
            Debug.DrawLine(ray.origin, hit.point);
            
            target = hit.collider.gameObject;
            
            interactableComponent = GetInteractableObjectOrParent(target);
        }
        else
        {
            target = null;
            interactableComponent = null;
        }

        if (Input.GetKeyDown(KeyCode.E) && interactableComponent != null)
        {
            Debug.Log("Calling: " + interactableComponent.name);
            OnInteract?.Invoke(interactableComponent.name);
        }
    }

    GameObject GetInteractableObjectOrParent(GameObject target)
    {
        if (target.CompareTag("Interactable"))
        {
            return target;
        }

        Transform myself = target.transform;
        while (myself.parent != null)
        {
            myself = myself.parent;
            if (myself.CompareTag("Interactable"))
            {
                return myself.gameObject;
            }
        }
        return null;
    }
}
