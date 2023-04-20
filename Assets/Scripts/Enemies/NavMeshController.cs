using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float agentSpeed = 1f;
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    private bool isPlayerDead = false;
    private bool isRunning = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.destination = player.position;
        agent.speed = agentSpeed;
    }

    void Update()
    {
        animationsController();
    }

    private void animationsController()
    {
        StartIdle();
        if (!isPlayerDead)
        {
            if (agent.remainingDistance <= 1)
            {
                StopChase();
                StopWalk();
                Attack();
            }
            else if (agent.remainingDistance <= 10)
            {
                StopChase();
                StartWalk();
            }
            else if (agent.remainingDistance >= 11)
            {
                StopWalk();
                StartChase();
            }
            animator.SetFloat("Speed", agent.speed);
            agent.destination = player.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Health>().getLives() == 0)
            {
                playerDead();
            }
            else
            {
                animator.SetTrigger("Attack");
            }
        }
    }


    #region Animation Events
    private void StartChase()
    {
        // Chase mode
        Scream();
    }

    private void Scream()
    {
        if (!isRunning)
        {
            //Should be run when health is low so the enemy starts running towards the player.
            agent.speed = 0;
            animator.SetTrigger("Scream");
            //wait 2 seconds before running
            StartCoroutine(waitForScream());

            StartRun();
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack1");
    }

    private void StartRun()
    {
        //Starts running towards the player
        isRunning = true;
        animator.SetBool("isRunning", true);
        agent.speed = 2;
    }

    private void StopRun()
    {
        isRunning = false;
        animator.SetBool("isRunning", false);
        agent.speed = agentSpeed;
    }

    private void StartWalk()
    {
        agent.speed = agentSpeed;
        animator.SetBool("isWalking", true);
    }

    private void StopWalk()
    {
        animator.SetBool("isWalking", false);
        StartIdle();
    }

    private void StartIdle()
    {
        animator.SetBool("isIdle", true);
    }

    private void StopIdle()
    {
        animator.SetBool("isIdle", false);
    }

    IEnumerator waitForScream()
    {
        yield return new WaitForSeconds(2.2f);
    }

    void playerDead()
    {
        isPlayerDead = true;
        animator.SetFloat("Speed", 0);
        agent.isStopped = true;
    }

    void StopChase()
    {
        StopRun();
        agent.speed = agentSpeed;
    }

    #endregion

}
