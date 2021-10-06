using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public bool pausable = false;

    public GameObject pauseMenu;

    void Start()
    {
        Config.Setup();
    }

    void Update()
    {
        if (pausable && Input.GetKeyDown(KeyCode.Escape)) Pause();
    }

    public void Pause()
    {
        if (pauseMenu.GetComponent<Canvas>().enabled)
        {
            pauseMenu.GetComponent<Canvas>().enabled = false;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            pauseMenu.GetComponent<Canvas>().enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OpenMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Menu");
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Settings");
    }

    public void CloseSettings()
    {
        Config.Save();

        OpenMainMenu();
    }

    public void OpenCredits()
    {
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Credits");
    }
}
