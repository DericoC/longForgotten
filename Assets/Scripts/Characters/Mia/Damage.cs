using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Damage : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] GameObject blood;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject b = Instantiate(blood, other.transform.position, Quaternion.identity);
            other.GetComponent<Health>().LostLive(count);
            Destroy(b, 2);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(blood, other.transform.position, Quaternion.identity);
            other.GetComponent<Health>().LostLive(count);
        }
    }
}
