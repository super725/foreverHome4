using Unity.IO.LowLevel.Unsafe;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject PauseMenu;

    public bool ispaused;
    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (ispaused)
            {
                resumGame();
            }
            else
            {
                pauseGame();
            }
        }
        
    }

    public void pauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        ispaused = true;
    }

    public void resumGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        ispaused = false;
    }
}
