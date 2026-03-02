using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

    public bool canMove = false;

    // Track current rider so we can unparent when they leave the overlap area
    private Transform _currentRider;

    void Start()
    {
        TargetNextWaypoint();
    }

    void FixedUpdate()
    {
        //Detects the rider first so we know if the player is there
        DetectAndHandleRider();
        
        //Can only move if the rider and canMove is true
        if (canMove && _currentRider != null)
        {
            _elapsedTime += Time.deltaTime;

            float elapsedPercentage = _elapsedTime / _timetoWaypoint;
            elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
            transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);
            transform.rotation = Quaternion.Lerp(_previousWaypoint.rotation, _targetWaypoint.rotation, elapsedPercentage);

            // Detect player using an overlap box so characters that don't use physics collisions still get parented


            if (elapsedPercentage >= 1.0f)
            {
                TargetNextWaypoint();
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

        // Removed incorrect BoxCast; overlap checking handled every FixedUpdate in DetectAndHandleRider
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

