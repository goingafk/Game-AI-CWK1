using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public Transform target;
    float speed = 5;
    Vector3[] path;
    int targetIndex;
    Vector3 targetLastPosition; // Keeps track of the last known position of the target
    bool targetHasMoved = false; // Flag to indicate if the target has moved

    void Start()
    {
        targetLastPosition = target.position; // Initialize with the target's initial position
        StartCoroutine(UpdatePath()); // Start the coroutine to update the path
    }

    // Coroutine to periodically check for changes in the target's position and request a new path
    IEnumerator UpdatePath()
    {
        while (true)
        {
            // Check if the target has moved significantly
            if (Vector3.Distance(target.position, targetLastPosition) > 0.1f || targetHasMoved)
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound); // Request a new path to the current target position
                targetLastPosition = target.position; // Update the last known position of the target
                targetHasMoved = false; // Reset the movement flag
            }
            yield return new WaitForSeconds(0.5f); // Check for updates every 0.5 seconds
        }
    }

    // Callback method when a new path is found
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath"); // Stop the current path-following coroutine
            StartCoroutine("FollowPath"); // Start a new path-following coroutine
        }
    }

    // Coroutine to follow the path
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break; // Exit the coroutine if the end of the path is reached
                }
                currentWaypoint = path[targetIndex]; // Update the current waypoint to the next one in the path
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime); // Move towards the current waypoint
            yield return null; // Wait until the next frame
        }
    }

    // Draws the path in the scene view for debugging purposes
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
