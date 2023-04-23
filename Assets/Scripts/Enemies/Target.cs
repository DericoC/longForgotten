using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    [Range(0, 100)]
    [SerializeField] float health = 100f;
    private Animator animator;
    private NavMeshAgent agent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

    }

    public void TakeDamage(float damage)
    {
        if(health < damage)
        {
            health = 0;
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            animator.SetTrigger("isDead");
            agent.enabled = false;
            agent.speed = 0;
            Destroy(gameObject, 3);
        }
        //animator.SetTrigger("isHit");
    }

    private void Update()
    {
        GetComponentInChildren<TMPro.TextMeshPro>().text = "Health: " + health.ToString();
    }
}
