using UnityEngine;

public class tRuCkknockback : MonoBehaviour
{
    [SerializeField] private float hitForce = 15f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Send the Enemy's forward direction instead of position
                player.ApplyKnockback(transform.forward, hitForce);
            }
        }
    }
}
