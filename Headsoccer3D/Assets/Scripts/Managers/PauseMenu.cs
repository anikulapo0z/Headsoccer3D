using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [SerializeField] bool isPaused = false;
    [SerializeField] GameObject pauseMenu;

    private void Awake()
    {
        Instance = this;
    }


    public void PauseGame(bool _setActive)
    {
        if (_setActive == false)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;

            isPaused = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;

            isPaused = true;
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;

            isPaused = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;

            isPaused = true;
        }
    }

}
