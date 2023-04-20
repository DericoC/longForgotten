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
        StartIdle();
        if (!isPlayerDead)
        {
            if (agent.remainingDistance <= 1)
            {
                StopWalk();
                StopRun();
                Attack();
            }
            else
            {   //Should check if health is low instead
                if (agent.speed == 2)
                {
                    Scream();
                }
                else
                {
                    StartWalk();
                }
            }
            animator.SetFloat("Speed", agent.speed);
            //Should update other animator floats here
            agent.destination = player.position;
        }
    }

    private void Scream()
    {
        //Should be run when health is low so the enemy starts running towards the player.
        animator.SetTrigger("Scream");
        //wait 2 seconds before running
        StartCoroutine(waitForScream());

        StartRun();
    }

    private void Attack()
    {
        animator.SetTrigger("Attack1");
    }

    private void StartRun()
    {
        //Starts running towards the player
        agent.speed = 2;
        animator.SetBool("isRunning", true);
    }

    private void StopRun()
    {
        animator.SetBool("isRunning", false);
        agent.speed = agentSpeed;
    }

    private void StartWalk()
    {
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
        yield return new WaitForSeconds(2);
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

    void playerDead()
    {
        isPlayerDead = true;
        animator.SetFloat("Speed", 0);
        agent.isStopped = true;
    }
}
