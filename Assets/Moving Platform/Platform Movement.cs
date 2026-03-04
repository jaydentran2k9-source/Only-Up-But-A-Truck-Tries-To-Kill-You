using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;


public class PlatformMovement : MonoBehaviour
{
    [SerializeField]
    private WaypointPath _waypointPath;

    [SerializeField]
    private float _speed;

    // Overlap box settings to detect the player standing on the platform
    [SerializeField]
    private Vector3 _overlapExtents = new Vector3(3f, 2f, 4f);

    [SerializeField]
    private LayerMask _overlapMask = ~0; // default to everything; set to Player layer if available

    private int _targetWaypointIndex;


    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    private float _timetoWaypoint;
    private float _elapsedTime;

    // External triggers can set this to control when the platform starts moving (e.g., when the player steps on it)
    public bool canMove = false;
    

    // Track current rider so we can unparent when they leave the overlap area
    private Transform _currentRider;

    void Start()
    {
        TargetNextWaypoint();
    }



    void FixedUpdate()
    {
        DetectAndHandleRider();

        // 1. Determine if we should move forward (Player is on) or return (Player is off)
        bool shouldMoveForward = (_currentRider != null && _targetWaypointIndex < 2); // Stops at Waypoint 2
        bool shouldReturnHome = (_currentRider == null && transform.position != _waypointPath.GetWaypoint(0).position);

        if (canMove && (shouldMoveForward || shouldReturnHome))
        {
            // If returning home, override the target to Waypoint 0
            if (shouldReturnHome && _targetWaypointIndex != 0)
            {
                _targetWaypointIndex = 0;
                TargetNextWaypoint();
            }

            _elapsedTime += Time.deltaTime;
            float elapsedPercentage = Mathf.Clamp01(_elapsedTime / _timetoWaypoint);
            elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);

            transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);
            transform.rotation = Quaternion.Lerp(_previousWaypoint.rotation, _targetWaypoint.rotation, elapsedPercentage);

            if (elapsedPercentage >= 1.0f)
            {
                // If we are moving forward and haven't hit the stop, get next
                if (shouldMoveForward)
                {
                    TargetNextWaypoint();
                    Debug.Log($"Reached Waypoint {_targetWaypointIndex}, moving to next.");
                }
            }
        }
    }



    private void DetectAndHandleRider()
    {
        // OverlapBox uses half-extents; orientation is the platform rotation
        Collider[] hits = Physics.OverlapBox(transform.position, _overlapExtents, transform.rotation, _overlapMask, QueryTriggerInteraction.Ignore);

        Transform foundPlayer = null;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag("Player"))
            {
                foundPlayer = hits[i].transform;
                break;
            }
        }

        if (foundPlayer != null)
        {
            canMove = true;

            if (_currentRider != foundPlayer)
            {
                // Parent the player to the platform so they move with it
                foundPlayer.SetParent(transform);
                _currentRider = foundPlayer;
            }
        }
        else
        {
            // No player found in overlap box: unparent any previously parented rider
            if (_currentRider != null)
            {
                // Only unparent if the parent is still this transform (safety)
                if (_currentRider.parent == transform)
                    _currentRider.SetParent(null);
                _currentRider = null;
            }
        }
    }

    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timetoWaypoint = distanceToWaypoint / _speed;

        
    }

   
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    // Draw the overlap box in the editor for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Matrix4x4 prev = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, _overlapExtents * 2f);
        Gizmos.matrix = prev;
    }

}

