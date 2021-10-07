using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public bool pausable = false;

    public GameObject pauseMenu;
    public bool paused { get { return pauseMenu.GetComponent<Canvas>().enabled; } set { pauseMenu.GetComponent<Canvas>().enabled = value; } }

    void Start()
    {
        Main.Instance = this;

        Config.Setup();
    }

    void Update()
    {
        if (pausable && Input.GetKeyDown(KeyCode.Escape)) Pause();
    }

    public void Pause()
    {
        if (paused)
        {
            paused = false;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            paused = true;

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
