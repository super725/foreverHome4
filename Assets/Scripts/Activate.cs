using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActivateGun : MonoBehaviour
{
    public List<GameObject> gunsOnFloor;
    public List<GameObject> activeGuns;
    public float maxDistance = 2f;
    private int currentGunIndex = -1;

    public TextMeshProUGUI ammoText; // Reference to the ammo text UI element.
    public TextMeshProUGUI pickupText; // Reference to the pickup text UI element.

    void Start()
    {
        foreach (GameObject gun in activeGuns)
        {
            gun.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
            {
                GameObject hitObject = hit.collider.gameObject;
                int index = gunsOnFloor.IndexOf(hitObject);
                if (index != -1)
                {
                    if (currentGunIndex != -1)
                    {
                        activeGuns[currentGunIndex].SetActive(false);
                    }

                    activeGuns[index].SetActive(true);
                    currentGunIndex = index;

                    gunsOnFloor[index].SetActive(false);

                    UpdateAmmoText(); // Update the ammo text when switching guns.
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentGunIndex != -1)
            {
                activeGuns[currentGunIndex].SetActive(false);
                gunsOnFloor[currentGunIndex].transform.position = Camera.main.transform.position + Camera.main.transform.forward * maxDistance;
                gunsOnFloor[currentGunIndex].SetActive(true);

                currentGunIndex = -1;

                UpdateAmmoText(); // Update the ammo text when dropping a gun.
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (currentGunIndex != -1)
            {
                activeGuns[currentGunIndex].SetActive(false);
            }

            currentGunIndex++;
            if (currentGunIndex >= activeGuns.Count)
            {
                currentGunIndex = 0;
            }
            activeGuns[currentGunIndex].SetActive(true);

            UpdateAmmoText(); // Update the ammo text when switching guns.
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (currentGunIndex != -1)
            {
                activeGuns[currentGunIndex].SetActive(false);
            }

            currentGunIndex--;
            if (currentGunIndex < 0)
            {
                currentGunIndex = activeGuns.Count - 1;
            }
            activeGuns[currentGunIndex].SetActive(true);

            UpdateAmmoText(); // Update the ammo text when switching guns.
        }

        if (pickupText != null)
        {
            if (currentGunIndex == -1)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    int index = gunsOnFloor.IndexOf(hitObject);
                    if (index != -1)
                    {
                        pickupText.text = "Press E to pick up";
                    }
                    else
                    {
                        pickupText.text = "";
                    }
                }
                else
                {
                    pickupText.text = "";
                }
            }
            else
            {
                pickupText.text = "";
            }
        }
    }
    void UpdateAmmoText()
    {
        if (currentGunIndex != -1)
        {
            ShotgunDmg shotgun = activeGuns[currentGunIndex].GetComponent<ShotgunDmg>();
            if (shotgun != null && ammoText != null)
            {
                ammoText.text = "Ammo: " + shotgun.currentAmmo.ToString() + "/" + shotgun.maxAmmo.ToString();
            }
        }
        else
        {
            ammoText.text = ""; // Clear the ammo text when no gun is active.
        }
    }
}
