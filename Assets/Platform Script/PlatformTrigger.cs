using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlatformTrigger1 : MonoBehaviour
{
    // Attach this to the Platform GameObject (requires a Collider set as "Is Trigger").
    // Assign the tRucKspawner in the Inspector or leave empty to auto-find on Start.

    [Tooltip("Drag the tRucKspawner instance here. If left empty, the script will try to find one at Start.")]
    public tRucKspawner spawner;

    void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<tRucKspawner>();
        }

        print(spawner);
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
