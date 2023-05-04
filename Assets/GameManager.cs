using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void EndGame()
    {
        // Add any game-ending logic here (e.g., saving scores, showing end screen, etc.)
        Debug.Log("Game Ended");

        // Restart the scene or load a new scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}