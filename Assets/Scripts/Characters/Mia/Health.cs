using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [SerializeField] int lives = 5;
    [SerializeField] bool _protected = false;
    [SerializeField] float protectedTime = 0.5f;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] AudioClip[] hurtSounds;
    private AudioSource audioSource;
    private LF.LongForgotten.Character player;
    private LogicController logicController;
    public static bool dead = false;

    private void Start()
    {
        logicController = GameObject.FindWithTag("Logic").GetComponent<LogicController>();
        player = GameObject.FindWithTag("Player").GetComponent<LF.LongForgotten.Character>();
        audioSource = player.GetComponent<AudioSource>();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (logicController.Pause)
        {
            GetComponent<PlayerInput>().enabled = false;
        }
        else if (!logicController.Pause)
        {
            GetComponent<PlayerInput>().enabled = true;
        }

        CheckIfAlive();
    }

    private void CheckIfAlive()
    {
        if (lives > 0)
        {
            dead = false;
            GetComponent<PlayerInput>().enabled = true;
            gameOverPanel.SetActive(false);
        }
        else if (lives == 0)
        {
            dead = true;
            GetComponent<PlayerInput>().enabled = false;
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            player.CursorLocked = false;
            Time.timeScale = 0;
        }
    }

    public void LostLive(int damage)
    {
        if (!_protected && lives > 0)
        {
            lives -= damage;
            hurtSound();
            StartCoroutine(Protect());
        }
    }

    public int getLives()
    {
        return lives;
    }

    private void hurtSound() {
        audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)], 0.8f);
    }

    IEnumerator Protect()
    {
        _protected = true;
        yield return new WaitForSeconds(protectedTime);
        _protected = false;
    }
}
