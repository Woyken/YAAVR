using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class BotRigidbodyNavigator : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    public float waitAfterWalkToTargetMin = 0f;

    public float waitAfterWalkToTargetMax = 10f;
    public PlayerAnimationHandler animationHandler;

    public CharacterRagdollThrowable characterThrowable;

    private float restartMovingAt = 0f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        Collider collider1 = GetComponent<Collider>();
        if (collider1 != null)
        {
            foreach (Collider child in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(collider1, child);
            }
        }

        characterThrowable.onRagdollStateChanged += (s) =>
        {
            switch (s)
            {
                case RagdollState.AnimationComplete:
                    {
                        // Setting nextPosition doesn't work in this case. nextPosition can only handle moving on navmesh and has to have valid path from 1 point to another.
                        // tried moving between 2 layers on same mesh, didn't work, even though path exists between them
                        // It not only needs a path, also can't be too far. So Warp here is better since it can get us anywhere instantly.
                        // Only thing that it need to be "close enough" to the navmesh, then it "snaps" to it, which might not always look all that great.
                        navMeshAgent.updatePosition = true;
                        navMeshAgent.updateRotation = true;
                        navMeshAgent.updateUpAxis = true;
                        if (navMeshAgent.Warp(transform.position))
                        {
                            navMeshAgent.isStopped = false;
                            navMeshAgent.Warp(transform.position);
                        }
                        else
                        {
                            navMeshAgent.updatePosition = false;
                            navMeshAgent.updateRotation = false;
                            navMeshAgent.updateUpAxis = false;
                        }
                        break;
                    }
                case RagdollState.Ragdoll:
                    {
                        // Disable the navigation agent
                        if (!navMeshAgent.isStopped)
                            navMeshAgent.isStopped = true;
                        navMeshAgent.updatePosition = false;
                        navMeshAgent.updateRotation = false;
                        navMeshAgent.updateUpAxis = false;
                        break;
                    }
            }
        };

    }

    void Start()
    {
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        if (navMeshAgent.isActiveAndEnabled)
        {
            // Since agent is active, update our animator with current speed values.
            var localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);
            localVelocity.y = 0f;
            localVelocity.Normalize();
            animationHandler.MovementSpeedMultiplier = navMeshAgent.velocity.WithY(0).magnitude / navMeshAgent.speed;
            animationHandler.VelocityX = localVelocity.x;
            animationHandler.VelocityY = localVelocity.z;
        }

        // Check if we've reached the destination
        if (navMeshAgent.isOnNavMesh && !navMeshAgent.pathPending)
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
}
