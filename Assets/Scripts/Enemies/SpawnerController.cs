using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{

    public GameObject[] enemyPrefabs; // array of enemy prefabs to spawn
    public float spawnRadius = 50f; // maximum distance from the player to spawn enemies
    public int maxEnemiesPerSpawner = 3; // maximum number of enemies to spawn per spawner
    public float spawnDelay = 10f; // delay between spawns of the same spawner
    public float roundDelay = 10f; // delay between rounds
    private int currentRound = 1; // current round number

    private GameObject[] spawners; // array of game objects with the "ZombieSpawner" tag
    private GameObject player; // reference to the player object

    private void Start()
    {
        // find all game objects with the "ZombieSpawner" tag
        spawners = GameObject.FindGameObjectsWithTag("Spawner");

        // get a reference to the player object
        player = GameObject.FindGameObjectWithTag("Player");

        // start the first round
        StartCoroutine(SpawnEnemiesWithDelay());
    }

    private void Update()
    {
        // get a reference to the player object
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private IEnumerator SpawnEnemiesWithDelay()
    {
        // wait for the specified round delay before starting the round
        yield return new WaitForSeconds(roundDelay);

        // loop through each spawner and check if it's within the spawn radius of the player
        foreach (GameObject spawner in spawners)
        {
            if (Vector3.Distance(spawner.transform.position, player.transform.position) <= spawnRadius)
            {
                // spawn up to maxEnemiesPerSpawner enemies at the spawner's position
                for (int i = 0; i < maxEnemiesPerSpawner; i++)
                {
                    // get a random index for the enemy prefab array
                    int randomIndex = Random.Range(0, enemyPrefabs.Length);

                    // spawn a random enemy prefab at the spawner's position
                    Instantiate(enemyPrefabs[randomIndex], spawner.transform.position, Quaternion.identity);

                    // wait for the specified spawn delay before spawning the next enemy
                    yield return StartCoroutine(SpawnDelay(spawnDelay));
                }
            }
        }

        // wait until all enemies are killed before starting the next round
        while (GameObject.FindWithTag("Zombie") != null)
        {
            yield return null;
        }

        // increment the current round
        currentRound++;
        Debug.Log(currentRound);
        //increase difficulty
        DifficultyIncrease();

        // start the next round
        StartCoroutine(SpawnEnemiesWithDelay());
    }

    private void DifficultyIncrease()
    {
        //increase the amount of zombies spawning per spawner each round
        maxEnemiesPerSpawner = currentRound * maxEnemiesPerSpawner;
    }

    private IEnumerator SpawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    //Getter / Setter
    public int CurrentRound { get => currentRound; set => currentRound = value; }
}
