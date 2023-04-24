using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    //ESTO ES PARA QUE SE VEA UN SLIDER EN EL EDITOR
    [Range(0, 100)]
    [SerializeField] float health = 100f;
    private Animator animator;
    private NavMeshAgent agent;

    private void Start()
    {
        //Traigo el componente animator para manejar las animaciones
        animator = GetComponent<Animator>();
        //Traigo el componente agent para manejar el AI
        agent = GetComponent<NavMeshAgent>();
    }

    //Funcion para recibir daño, no deja que la vida sea menor a 0 y maneja las animaciones y la muerte
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

    //Funcion para mostrar la vida en pantalla
    private void Update()
    {
        GetComponentInChildren<TMPro.TextMeshPro>().text = "Health: " + health.ToString();
    }
}
