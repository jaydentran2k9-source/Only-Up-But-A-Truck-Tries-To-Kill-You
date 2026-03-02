using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    // Drag your platform here in the Inspector
    [SerializeField] private PlatformMovement _platform;

    // Stop movement when player enters
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _platform.canMove = false;
        }
    }

    // Resume movement when player leaves
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _platform.canMove = true;
        }
    }
}
