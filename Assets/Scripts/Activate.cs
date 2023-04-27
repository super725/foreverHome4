using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGun : MonoBehaviour
{
    public List<GameObject> gunsOnFloor; // References to the guns on the floor.
    public List<GameObject> activeGuns; // References to the active guns.
    public float maxDistance = 2f; // Maximum distance to check for the guns on the floor.
    private int currentGunIndex = -1;

    
void Start()
    {
        // Deactivate all the active guns at the start of the game.
        foreach (GameObject gun in activeGuns)
        {
            gun.SetActive(false);
        }
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
                int index = gunsOnFloor.IndexOf(hitObject);
                if (index != -1)
                {
                    // If the player picks up a second gun, deactivate the first gun.
                    if (currentGunIndex != -1)
                    {
                        activeGuns[currentGunIndex].SetActive(false);
                    }

                    // Activate the new gun and update the current gun index.
                    activeGuns[index].SetActive(true);
                    currentGunIndex = index;

                    // Deactivate the gun on the floor.
                    gunsOnFloor[index].SetActive(false);
                }
            }
        }

        // Check if the player is holding a gun and press "G" to drop it on the floor.
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentGunIndex != -1)
            {
                // Deactivate the current gun and drop it on the floor.
                activeGuns[currentGunIndex].SetActive(false);
                gunsOnFloor[currentGunIndex].transform.position = Camera.main.transform.position + Camera.main.transform.forward * maxDistance;
                gunsOnFloor[currentGunIndex].SetActive(true);

                // Update the current gun index to -1.
                currentGunIndex = -1;
            }
        }

        // Check if the player wants to switch to the other gun using the mouse wheel.
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // Deactivate the current gun.
            if (currentGunIndex != -1)
            {
                activeGuns[currentGunIndex].SetActive(false);
            }

            // Update the current gun index and activate the new gun.
            currentGunIndex++;
            if (currentGunIndex >= activeGuns.Count)
            {
                currentGunIndex = 0;
            }
            activeGuns[currentGunIndex].SetActive(true);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // Deactivate the current gun.
            if (currentGunIndex != -1)
            {
                activeGuns[currentGunIndex].SetActive(false);
            }

            // Update the current gun index and activate the new gun.
            currentGunIndex--;
            if (currentGunIndex < 0)
            {
                currentGunIndex = activeGuns.Count - 1;
            }
            activeGuns[currentGunIndex].SetActive(true);
        }
    }
}