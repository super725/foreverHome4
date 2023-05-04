using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd: MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        // Add any game-ending logic here (e.g., saving scores, showing end screen, etc.)
        Debug.Log("Game Ended");

        // Restart the scene or load a new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}

