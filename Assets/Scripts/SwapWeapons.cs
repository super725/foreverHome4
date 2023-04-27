using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapWeapons : MonoBehaviour
{
    public List<GameObject> gunsOnFloor; // References to the guns on the floor.
    private List<GameObject> activeGuns; // References to the active guns.
    private int currentGunIndex = 0; // Index of the currently active gun.
    public float maxDistance = 2f; // Maximum distance to check for the guns on the floor.

    private void Start()
    {
        activeGuns = new List<GameObject>();
    }

    void Update()
    {
        // Check if the player is looking at a gun on the floor and press "E" to activate it.
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (activeGuns.Count < 2 && gunsOnFloor.Contains(hitObject))
                {
                    hitObject.SetActive(false);
                    activeGuns.Add(hitObject);
                    currentGunIndex = activeGuns.Count - 1;
                    activeGuns[currentGunIndex].SetActive(true);
                }
            }
        }

        // Check if the player is holding a gun and press the mouse wheel to switch to the other gun.
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && activeGuns.Count == 2)
        {
            activeGuns[currentGunIndex].SetActive(false);
            currentGunIndex = (currentGunIndex + 1) % 2;
            activeGuns[currentGunIndex].SetActive(true);
        }

        // Check if the player is holding a gun and press "G" to drop it on the floor.
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (activeGuns.Count > 0)
            {
                GameObject activeGun = activeGuns[currentGunIndex];
                activeGun.SetActive(false);
                activeGuns.RemoveAt(currentGunIndex);
                gunsOnFloor.Add(activeGun);
                currentGunIndex = activeGuns.Count > 0 ? 0 : -1;
            }
        }
    }
}