using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nav : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float agentSpeed;
    private Animator animator;
    private NavMeshAgent agent;
    private bool isPlayerDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.destination = player.position;
        agent.speed = agentSpeed;
    }

    void Update()
    {
        if (!isPlayerDead)
        {
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

    void playerDead()
    {
        isPlayerDead = true;
        animator.SetFloat("Speed", 0);
        agent.isStopped = true;
    }
}
