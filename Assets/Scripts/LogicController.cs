using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    private LF.LongForgotten.Character player;
    private GameObject characterCanvas;
    private bool pause;

    private void Start()
    {
        characterCanvas = GameObject.FindWithTag("PlayerGUI");
        player = GameObject.FindWithTag("Player").GetComponent<LF.LongForgotten.Character>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Health.dead)
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
        characterCanvas.SetActive(false);
        player.CursorLocked = false;
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
        characterCanvas.SetActive(true);
        player.CursorLocked = true;
        pause = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Getter / Setter
    public bool Pause { get => pause; set => pause = value; }
}
