using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] int lives = 3;
    [SerializeField] bool _protected = false;
    [SerializeField] float protectedTime = 1;
    [SerializeField] float waitTime = 0.2f;
    private LogicController logicController;
    private Animator animator;
    public bool dead = false;
    private GameObject gameOverPanel;

    private void Start()
    {
        logicController = GameObject.FindWithTag("Logic").GetComponent<LogicController>();
        animator = GetComponent<Animator>();
        gameOverPanel = GameObject.FindWithTag("GameOver");
    }

    private void Update()
    {
        if (!logicController.Pause) {
            if (lives == 0)
            {
                animator.SetBool("Dead", true);
                animator.SetFloat("Speed", 0);
                GetComponent<MiaScript>().enabled = false;
                gameOverPanel.SetActive(true);
            }
        }
    }

    public void LostLive(int damage)
    {
        if (!_protected && lives > 0)
        {
            animator.SetTrigger("Hit");
            lives -= damage;
            StartCoroutine(Protect());
            //StartCoroutine(StopVelocity());
        }
    }

    public int getLives()
    {
        return lives;
    }

    public void restartGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void resume()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator Protect()
    {
        _protected = true;
        yield return new WaitForSeconds(protectedTime);
        _protected = false;
    }

    //IEnumerator StopVelocity()
    //{
    //    var actualVelocity = GetComponent<MiaScript>().PlayerSpeed;
    //    GetComponent<MiaScript>().PlayerSpeed = 0;
    //    yield return new WaitForSeconds(waitTime);
    //    GetComponent<MiaScript>().PlayerSpeed = actualVelocity;
    //}
}
