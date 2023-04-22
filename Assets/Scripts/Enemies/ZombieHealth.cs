using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Range(0, 100)]
    public float Health = 100.0f;
    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        GetComponentInChildren<TMPro.TextMeshPro>().text = "Health: " + Health.ToString();
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
