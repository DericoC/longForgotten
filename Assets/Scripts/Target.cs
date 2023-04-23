using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    [Range(0, 100)]
    [SerializeField] float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);   
    }

    private void Update()
    {
        GetComponentInChildren<TMPro.TextMeshPro>().text = "Health: " + health.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10f);
        }
        if(collision.gameObject.CompareTag("Punch"))
        {
            TakeDamage(5f);
        }
    }
}
