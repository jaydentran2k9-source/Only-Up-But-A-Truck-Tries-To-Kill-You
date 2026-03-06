using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlatformTrigger1 : MonoBehaviour
{
    // Attach this to the Platform GameObject (requires a Collider set as "Is Trigger").
    // Assign the tRucKspawner in the Inspector or leave empty to auto-find on Start.

    [Tooltip("Drag the tRucKspawner instance here. If left empty, the script will try to find one at Start.")]
    public tRucKspawner spawner;

    public Vector3 velocity {get; private set; }
    private Vector3 lastPosition;
    private Rigidbody rb;

    void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<tRucKspawner>();
        }

        print(spawner);

        rb = GetComponent<Rigidbody>();
        lastPosition = rb.position;
    }

    void FixedUpdate()
    {
        // Calculate velocity: (Current Position - Last Position) / Time
        velocity = (rb.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = rb.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && spawner != null)
        {
            // Player entered platform: stop spawning
            spawner.SetSpawning(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && spawner != null)
        {
            // Player left platform: resume spawning
            spawner.SetSpawning(true);
        }
    }
}
