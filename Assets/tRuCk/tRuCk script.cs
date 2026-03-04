using UnityEngine;
using UnityEngine.EventSystems;

public class tRuCkscript : MonoBehaviour
{
    public float speed = 10f;
    public float lifeSpan = 10f;
    private Vector3 moveDirection;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //Set y to 0 so the truck doesn't fly up/down if the player jumps
        // Calculate the direction from the truck to the player (Playerposition - Truckposition)
        if (player != null)
        {
            moveDirection = (player.transform.position - transform.position);
            moveDirection.y = 0; // Keep the truck on the same horizontal plane
            moveDirection.Normalize();
        }

        // 3. Make the truck face the player
        transform.rotation = Quaternion.LookRotation(moveDirection);

        // Automatically remove the truck after 10 seconds
        Destroy(gameObject, lifeSpan);
    }

    // Update is called once per frame
    void Update()
    {
        // 5. Move straight forward in that direction
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
