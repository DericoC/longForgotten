using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] GameObject blood;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameObject BloodParticle = Instantiate(blood, collider.transform.position, Quaternion.identity);
            collider.GetComponent<Health>().LostLive(count);
            Destroy(BloodParticle, 0.3f);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameObject BloodParticle = Instantiate(blood, collider.transform.position, Quaternion.identity);
            collider.GetComponent<Health>().LostLive(count);
            Destroy(BloodParticle, 0.3f);
        }
    }
}
