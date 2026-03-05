using UnityEngine;

public class tRucKspawner : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public GameObject objectToSpawnPrefab; // Reference to the prefab to instantiate
    public GameObject Platform;
    public float spawnDistance = 10f; // The set distance from the player
    public float spawnInterval = 5f; // Time between spawns

    private float timer;
    public int maxtruKs = 1; // Maximum number of trucks allowed in the scene

    // Flag to control spawning. When false, spawning is paused.
    public bool isSpawning = true;

    // Cached colliders used to determine whether player is touching the platform
    private Collider playerCollider;
    private Collider platformCollider;

    void Start()
    {
        // Default: allow spawning. PlatformTrigger will disable when player enters a platform.
        isSpawning = true;

        if (player != null)
            playerCollider = player.GetComponent<Collider>();

        if (Platform != null)
            platformCollider = Platform.GetComponent<Collider>();

        if (playerCollider == null)
            Debug.LogWarning("tRucKspawner: player has no Collider component assigned or found.");

        if (Platform != null && platformCollider == null)
            Debug.LogWarning("tRucKspawner: Platform has no Collider component assigned or found.");
    }

    void Update()
    {
        // If spawning is paused, do nothing and do not accumulate timer.
        if (!isSpawning)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            print(CanSpawn());
            if (GameObject.FindGameObjectsWithTag("tRUck").Length < maxtruKs && CanSpawn())
            {
                SpawnObject();
            }

            timer -= spawnInterval;
        }
    }

    // Public API for external callers (e.g. PlatformTrigger) to enable/disable spawning.
    public void SetSpawning(bool enable)
    {
        isSpawning = enable;
        if (!isSpawning)
        {
            // Reset timer so there is no immediate spawn when re-enabled.
            timer = 0f;
        }
    }

    // Returns true only when the player IS touching the specified Platform.
    public bool CanSpawn()
    {
        // Require both colliders to be present. If missing, don't allow spawn because requirement is
        // "only spawn if the player is touching the Platform".
        if (playerCollider == null || platformCollider == null)
            return false;

        // Only allow spawning when the colliders' bounds intersect (considered "touching").
        return playerCollider.bounds.Intersects(platformCollider.bounds);
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
        Instantiate(objectToSpawnPrefab, spawnPosition, Quaternion.identity);
    }
}


