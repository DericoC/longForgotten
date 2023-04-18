using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] GameObject dust;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(dust, other.transform.position, Quaternion.identity);
            other.GetComponent<Health>().LostLive(count);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(dust, other.transform.position, Quaternion.identity);
            other.GetComponent<Health>().LostLive(count);
        }
    }
}
