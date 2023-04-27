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
    [SerializeField] AudioClip[] hurtSounds;
    private bool soundPlaying = false;
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
        }
        else if (lives == 0)
        {
            dead = true;
            GetComponent<PlayerInput>().enabled = false;
            logicController.gameOver();
        }
    }

    public void LostLive(int damage)
    {
        if (!_protected && lives > 0)
        {
            lives -= damage;
            if (!soundPlaying) {
                StartCoroutine(hurtSound());
            }
            StartCoroutine(Protect());
        }
    }

    public int getLives()
    {
        return lives;
    }

    IEnumerator hurtSound() {
        soundPlaying = true;
        audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)], 0.8f);
        yield return new WaitForSeconds(2f);
        soundPlaying = false;
    }

    IEnumerator Protect()
    {
        _protected = true;
        yield return new WaitForSeconds(protectedTime);
        _protected = false;
    }
}
