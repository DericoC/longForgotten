using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    private bool pause;
    private GameObject cross;

    private void Start()
    {
        cross = GameObject.FindWithTag("Cross");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            if (pause)
            {
                resume();
            }
            else
            {
                mapPause();
            }
        }
    }

    public void exitGame()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void mapPause()
    {
        cross.SetActive(false);
        Time.timeScale = 0;
        pause = true;
        pauseMenu.SetActive(true);
    }

    public void restartGame()
    {
        pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void resume()
    {
        pauseMenu.SetActive(false);
        cross.SetActive(true);
        pause = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void resumeTimeScale()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    //Getter / Setter
    public bool Pause { get => pause; set => pause = value; }
}
