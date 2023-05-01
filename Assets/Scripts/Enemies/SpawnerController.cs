using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{

    public GameObject[] enemyPrefabs; // array of enemy prefabs to spawn
    public float spawnRadius = 50f; // maximum distance from the player to spawn enemies
    public int maxEnemiesPerSpawner = 1; // maximum number of enemies to spawn per spawner
    public float spawnDelay = 10f; // delay between spawns of the same spawner
    public float roundDelay = 10f; // delay between rounds
    public static int currentRound; // current round number

    private GameObject[] spawners; // array of game objects with the "ZombieSpawner" tag
    private GameObject player; // reference to the player object
    public static int zombies;
    private bool roundStarted;

    private void Start()
    {
        currentRound = 1;
        zombies = 0;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnEnemiesWithDelay(true));
    }

    private void Update()
    {
        // get a reference to the player object
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (roundStarted) {
            roundEndChecker();
        }
    }

    private IEnumerator SpawnEnemiesWithDelay(bool firstRound)
    {
        roundStarted = false;
        int maxZombieRound = maxPerRound();
        bool maxReached = false;

        yield return new WaitForSeconds(firstRound ? 2f : roundDelay);
        foreach (GameObject spawner in spawners)
        {
            if (Vector3.Distance(spawner.transform.position, player.transform.position) <= spawnRadius)
            {
                for (int i = 0; i < maxEnemiesPerSpawner; i++)
                {
                    // get a random index for the enemy prefab array
                    int randomIndex = Random.Range(0, enemyPrefabs.Length);

                    // spawn a random enemy prefab at the spawner's position
                    Instantiate(enemyPrefabs[randomIndex], spawner.transform.position, Quaternion.identity);
                    zombies++;

                    if (zombies == maxZombieRound) {
                        maxReached = true;
                        break;
                    }

                    // wait for the specified spawn delay before spawning the next enemy
                    yield return StartCoroutine(SpawnDelay(1.5f));
                }
            }

            if (maxReached)
            {
                break;
            }
        }
        roundStarted = true;
    }

    private void roundEndChecker() {
        if (zombies == 0) {
            currentRound++;
            DifficultyIncrease();
            StartCoroutine(SpawnEnemiesWithDelay(false));
        }
    }

    private void DifficultyIncrease()
    {
        maxEnemiesPerSpawner++;
    }

    private int maxPerRound() {
        int max = System.Convert.ToInt32(Mathf.Floor((maxEnemiesPerSpawner * spawners.Length) / 3.5f));
        return max;
    }

    private IEnumerator SpawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    //Getter / Setter
    public int CurrentRound { get => currentRound; set => currentRound = value; }
}
