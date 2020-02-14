using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class BotRigidbodyNavigator : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent navMeshAgent;

    public float waitAfterWalkToTargetMin = 0f;

    public float waitAfterWalkToTargetMax = 10f;

    private float restartMovingAt = 0f;
    private float lastTimeInputReceived = 0f;
    private bool isBeingHeld = false;
    private bool isSliding = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        if (isBeingHeld)
        {
            return;
        }
        if (navMeshAgent.isOnNavMesh)
            rb.isKinematic = true;

        if (!navMeshAgent.isOnNavMesh)
        {
            // We have been dropped off nav mesh.
            // Try to reatach it to warping to same location as we are right now.
            // When sliding is implemented, this should only happen after sliding is done.
            // navMeshAgent.nextPosition = hit.position;
            // Setting nextPosition doesn't work in this case. nextPosition can only handle moving on navmesh and has to have valid path from 1 point to another.
            // tried moving between 2 layers on same mesh, didn't work, even though path exists between them
            // It not only needs a path, also can't be too far. So Warp here is better since it can get us anywhere instantly.
            // Only thing that it need to be "close enough" to the navmesh, then it "snaps" to it, which might not always look all that great.
            navMeshAgent.Warp(rb.position);
        }
        // Check if we've reached the destination
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // Path is done. Set set new random destination.
                    SetNewDestination();
                }
            }
        }
    }

    private void SetNewDestination()
    {
        if (restartMovingAt < Time.time && !navMeshAgent.isStopped)
        {
            var waitFor = Random.Range(waitAfterWalkToTargetMin, waitAfterWalkToTargetMax);
            restartMovingAt = Time.time + waitFor;
            navMeshAgent.isStopped = true;
            return;
        }
        if (restartMovingAt > Time.time)
        {
            return;
        }

        var dest = RandomNavmeshLocation(4f);
        navMeshAgent.SetDestination(dest);
        navMeshAgent.isStopped = false;
        return;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere.WithY(0) * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public void OnHandPickup()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        isBeingHeld = true;
    }

    public void OnHandRelease()
    {
        // Disable navigation agent
        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = true;
        isBeingHeld = false;

        // Might not be nescessary after sliding after thrown is implemented. Then it would be called after sliding is done.
        // Set navigator current position to current body location.
        // This can be off navigation mesh. This means navigator won't work at all.
        // Will need to be "Warped" again to valid location on nav mesh
        navMeshAgent.Warp(rb.position);

        // Enable rigidbody physics
        rb.isKinematic = false;
    }
}
