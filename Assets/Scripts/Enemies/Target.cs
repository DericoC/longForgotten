using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class Target : MonoBehaviour {

    public int health = 100;
    public bool isHit = false;
    public bool isDead = false;

    [Header("Protection")]
    public float protectionTime;
    public bool protectionActive = false;

    [Header("Audio")]
    public AudioClip damageSound;

    private AudioSource audioSource;
    private NavMeshController controller;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<NavMeshController>();
    }

    private void Update()
    {
        if (!isDead) {
            if (isHit == true)
            {
                if (!protectionActive) {
                    StartCoroutine(protection());
                    health -= 20;
                    audioSource.GetComponent<AudioSource>().clip = damageSound;
                    audioSource.Play();

                    if (health <= 0)
                    {
                        isDead = true;
                    }
                    isHit = false;
                }
            }
        } else {
            controller.triggerDead();
            StartCoroutine(deleteBody());
        }
    }

    IEnumerator protection() {
        protectionActive = true;
        yield return new WaitForSeconds(protectionTime);
        protectionActive = false;
    }

    IEnumerator deleteBody() {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
}
