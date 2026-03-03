using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointPath : MonoBehaviour
{
    //public int _stopWaypointIndex = 2; // Set limit here

    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        int nextWaypointIndex = currentWaypointIndex + 1;
        if (nextWaypointIndex >= transform.childCount)
        {
            return 0; // Loop back to the first waypoint
        }
        return nextWaypointIndex;
    }

    //public int ResetToStart()
    //{
    //    return 0;
    //}
}
