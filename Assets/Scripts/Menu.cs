using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu: MonoBehaviour
{
    [SerializeField]private GameObject settingsMenu, pauseMenu;
    private bool isPaused = false;
    [SerializeField] private bool isMainMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (isMainMenu) isPaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isMainMenu)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
            isPaused = !isPaused;
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Play()
    {
        SceneManager.LoadScene("Level1"); 
    }
    public void LoadSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void LeaveSettings()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
