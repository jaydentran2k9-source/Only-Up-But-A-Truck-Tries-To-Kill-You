using UnityEngine;

public class tRucKspawner : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public GameObject objectToSpawnPrefab; // Reference to the prefab to instantiate
    public float spawnDistance = 10f; // The set distance from the player
    public float spawnInterval = 5f; // Time between spawns

    private float timer;
    public int maxtruKs = 1; // Maximum number of trucks allowed in the scene

    


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            
            if (GameObject.FindGameObjectsWithTag("tRUck").Length < maxtruKs)
            {
                SpawnObject();
            }

            timer -= spawnInterval;
        }
    }

    void SpawnObject()
    {
        // 1. Calculate a random direction from the player
        Vector3 randomDirection = Random.insideUnitSphere;

        randomDirection.y = 0; // Keeps the truck spawning at the same height as the player
        randomDirection.Normalize();

        // 2. Calculate the spawn position
        Vector3 spawnPosition = player.transform.position + randomDirection * spawnDistance;



        // 3. Instantiate the object
        // Use Quaternion.identity for no rotation, or specify a rotation as needed
        Instantiate(objectToSpawnPrefab, spawnPosition, Quaternion.identity);
    }
}