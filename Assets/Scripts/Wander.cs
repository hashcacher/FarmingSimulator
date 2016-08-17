using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour
{
    public float forcedSpeed;
    public bool pause;
    public float maxPauseDuration;
    public float destinationTimeout;

    private Vector3 forcedDestination, distanceVector;
    private float distance;
    private float timeReceivedDestination;

    private bool paused;
    private float pauseTime;
    private float pauseDuration;

    void Awake()
    {
        pauseDuration = 3;
    }

    void Update()
    {
        float time = Time.time;

        if (pauseTime + pauseDuration > time && time > pauseTime)
        {
            paused = true;
            return;
        }
        else if(paused)
        {
            paused = false;
            pauseTime = time + Random.Range(maxPauseDuration/2, maxPauseDuration);
            pauseDuration = Random.Range(0, maxPauseDuration);
        }

        // Reached our destination?
        if (distance < 1f || time > timeReceivedDestination + destinationTimeout)
        {
            // Get new destination        
            NextDestination(time, GetRandomDestination(24));
        }

        // Get agent distance from destination
        distanceVector = forcedDestination - transform.position;
        distance = new Vector3(distanceVector.x, 0, distanceVector.z).magnitude;

        // Set velocity
        Vector3 velocity = distanceVector.normalized * forcedSpeed;
        velocity.y = 0;
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // Gradually rotate
        Vector3 targetRotation = Quaternion.LookRotation(distanceVector).eulerAngles;
        targetRotation.x = transform.rotation.x;
        targetRotation.z = transform.rotation.z;
        transform.rotation =
            Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * 3f);
    }

    private void NextDestination(float time, Vector3 dest)
    {
        // Get new destination        
        forcedDestination = dest;
        timeReceivedDestination = time;
    }

    private Vector3 GetRandomDestination(float maxRange)
    {
        for (int i = 0; i < 30; i++)
        {
            float range = Random.Range(maxRange / 4, maxRange);

            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;

            // Check that it's not through a wall and it's on the map
            if (!Physics.Raycast(transform.position + Vector3.up * .1f, randomDirection, range) && 
                Physics.Raycast(transform.position + Vector3.up * .1f + randomDirection * range, Vector3.down, .2f))
            {
                return transform.position + randomDirection * range;
            }
        }

        return transform.position + Random.insideUnitSphere * .1f;
    }
}
