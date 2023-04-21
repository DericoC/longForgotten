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
    [SerializeField] GameObject gameOverPanel;
    private LogicController logicController;
    private Animator animator;
    public static bool dead = false;

    private void Start()
    {
        logicController = GameObject.FindWithTag("Logic").GetComponent<LogicController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!logicController.Pause) {
            if (lives == 0)
            {
                dead = true;
                animator.SetBool("Dead", true);
                animator.SetFloat("XSpeed", 0);
                animator.SetFloat("YSpeed", 0);
                GetComponent<MiaScript>().enabled = false;
                gameOverPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
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
}
