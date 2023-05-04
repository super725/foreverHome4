using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public FirstPersonController firstPersonController;
    public float blurIntensity = 1f;

    private bool isPaused = false;
    private Vector3 originalCameraPosition;
    private RawImage blurImage;
    private Texture2D blurredTexture;
    private Color[] originalPixels;

    private void Start()
    {
        // Create a RawImage component for the blur effect
        GameObject blurObject = new GameObject("BlurImage");
        blurObject.transform.SetParent(transform);
        blurImage = blurObject.AddComponent<RawImage>();
        blurImage.rectTransform.SetParent(transform);
        blurImage.rectTransform.anchorMin = Vector2.zero;
        blurImage.rectTransform.anchorMax = Vector2.one;
        blurImage.rectTransform.offsetMin = Vector2.zero;
        blurImage.rectTransform.offsetMax = Vector2.zero;

        // Set up the blur image
        blurredTexture = new Texture2D(Screen.width, Screen.height);
        blurImage.texture = blurredTexture;
        blurImage.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);

        // Disable FirstPersonController
        firstPersonController.enabled = false;

        // Save the original camera position
        originalCameraPosition = firstPersonController.transform.position;

        // Take a screenshot and apply the blur effect
        originalPixels = ScreenCapture.CaptureScreenshotAsTexture().GetPixels();
        ApplyBlurEffect();

        // Enable the blur image
        blurImage.enabled = true;
    }

    private void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);

        // Enable FirstPersonController
        firstPersonController.enabled = true;

        // Reset camera position
        firstPersonController.transform.position = originalCameraPosition;

        // Disable the blur image
        blurImage.enabled = false;
    }

    private void ApplyBlurEffect()
    {
        int blurSize = (int)(blurIntensity * 0.1f * Screen.width);
        int width = Screen.width;
        int height = Screen.height;

        Color[] blurredPixels = new Color[originalPixels.Length];

        for (int x = blurSize; x < width - blurSize; x++)
        {
            for (int y = blurSize; y < height - blurSize; y++)
            {
                Color blurredColor = Color.black;

                for (int dx = -blurSize; dx <= blurSize; dx++)
                {
                    for (int dy = -blurSize; dy <= blurSize; dy++)
                    {
                        int pixelIndex = (y + dy) * width + (x + dx);
                        blurredColor += originalPixels[pixelIndex];
                    }
                }

                blurredPixels[y * width + x] = blurredColor / ((blurSize * 2 + 1) * (blurSize * 2 + 1));
            }
        }

        blurredTexture.SetPixels(blurredPixels);
        blurredTexture.Apply();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
