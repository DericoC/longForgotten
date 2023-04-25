using System;
using UnityEngine;
using System.Collections;
using LF.LongForgotten;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter;
	[Tooltip("If enabled the bullet destroys on impact")]
	public bool destroyOnImpact = false;
	[Tooltip("Minimum time after impact that the bullet is destroyed")]
	public float minDestroyTime;
	[Tooltip("Maximum time after impact that the bullet is destroyed")]
	public float maxDestroyTime;

	[Header("Impact Effect Prefabs")]
	public Transform [] bloodImpactPrefabs;
	public Transform [] metalImpactPrefabs;
	public Transform [] dirtImpactPrefabs;
	public Transform []	concreteImpactPrefabs;

	private void Start ()
	{
		//Grab the game mode service, we need it to access the player character!
		var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
		//Ignore the main player character's collision. A little hacky, but it should work.
		Physics.IgnoreCollision(gameModeService.GetPlayerCharacter().GetComponent<Collider>(), GetComponent<Collider>());
		
		//Start destroy timer
		StartCoroutine (DestroyAfter ());
	}

	//If the bullet collides with anything
	private void OnCollisionEnter(Collision collision)
	{
		//Ignore collisions with other projectiles.
		if (collision.gameObject.GetComponent<Projectile>() != null)
			return;

		//If destroy on impact is false, start 
		//coroutine with random destroy timer
		if (!destroyOnImpact)
		{
			StartCoroutine(DestroyTimer());
		}
		//Otherwise, destroy bullet on impact
		else
		{
			Destroy(gameObject);
		}

		switch (collision.transform.tag)
		{
			case "Blood":
				// Instantiate random impact prefab from array
				Instantiate(bloodImpactPrefabs[Random.Range(0, bloodImpactPrefabs.Length)],
					transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
				break;

			case "Metal":
				// Instantiate random impact prefab from array
				Instantiate(metalImpactPrefabs[Random.Range(0, metalImpactPrefabs.Length)],
					transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
				break;

			case "Dirt":
				// Instantiate random impact prefab from array
				Instantiate(dirtImpactPrefabs[Random.Range(0, dirtImpactPrefabs.Length)],
					transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
				break;

			case "Concrete":
				// Instantiate random impact prefab from array
				Instantiate(concreteImpactPrefabs[Random.Range(0, concreteImpactPrefabs.Length)],
					transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
				break;

			case "Target":
				// Toggle "isHit" on target object
				collision.transform.gameObject.GetComponent<TargetScript>().isHit = true;
				break;

			case "Zombie":
				// Instantiate random impact prefab from array
				Instantiate(bloodImpactPrefabs[Random.Range(0, bloodImpactPrefabs.Length)],
					transform.position, Quaternion.LookRotation(collision.contacts[0].normal));

				collision.transform.gameObject.GetComponent<Target>().isHit = true;
				break;

			case "ExplosiveBarrel":
				// Toggle "explode" on explosive barrel object
				collision.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
				break;

			case "GasTank":
				// Toggle "isHit" on gas tank object
				collision.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
				break;

			default:
                // Instantiate random impact prefab from array
                Instantiate(concreteImpactPrefabs[Random.Range(0, concreteImpactPrefabs.Length)],
                    transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
                break;
		}
        // Destroy bullet object
        Destroy(gameObject);
    }

        private IEnumerator DestroyTimer () 
	{
		//Wait random time based on min and max values
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		//Destroy bullet object
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter () 
	{
		//Wait for set amount of time
		yield return new WaitForSeconds (destroyAfter);
		//Destroy bullet object
		Destroy (gameObject);
	}
}