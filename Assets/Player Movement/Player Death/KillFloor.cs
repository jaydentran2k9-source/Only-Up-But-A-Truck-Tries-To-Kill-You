using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class KillFloor : MonoBehaviour
{
    [SerializeField] private Transform respawn_point;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().enabled = false;
            other.transform.position = respawn_point.transform.position;
            other.GetComponent<PlayerMovement>().enabled = true;
        }
    }
}
