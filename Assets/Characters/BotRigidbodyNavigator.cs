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
    private new Rigidbody rigidbody;

    public float waitAfterWalkToTargetMin = 0f;

    public float waitAfterWalkToTargetMax = 10f;
    public PlayerAnimationHandler animationHandler;
    public RagdollHelper ragdollHelper;
    public Valve.VR.InteractionSystem.Throwable throwable;
    public Rigidbody rbForSpeedTracking;
    public Transform ragdollRoot;
    public Rigidbody ragdollHips;

    private float restartMovingAt = 0f;
    private float lastTimeInputReceived = 0f;
    private bool isBeingHeld = false;
    private bool isSliding = false;
    private bool isFalling = false;
    private bool isFallingLastFrame = false;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        Collider collider1 = GetComponent<Collider>();
        if (collider1 != null)
        {
            foreach (Collider child in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(collider1, child);
            }
        }
    }

    void Start()
    {
        throwable.onDetachFromHand.AddListener(OnHandRelease);
        //rb.isKinematic = true;
        ragdollHelper.onStateChanged += (s) =>
        {
            if (s == RagdollHelper.RagdollState.animated)
            {
                // Just finished the transition to animation.

                //                animationHandler.GetComponent<Animator>().applyRootMotion = false;

                // We have been dropped off nav mesh.
                // Try to reatach it to warping to same location as we are right now.
                // When sliding is implemented, this should only happen after sliding is done.
                // navMeshAgent.nextPosition = hit.position;
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
            }
        };
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

        if (isFalling)
        {
            UpdateWhenFalling();
            return;
        }

        if (navMeshAgent.isOnNavMesh)
        {
            // Since agent is on ground, update our animator with current speed values.
            var localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);
            localVelocity.y = 0f;
            localVelocity.Normalize();
            animationHandler.MovementSpeedMultiplier = navMeshAgent.velocity.WithY(0).magnitude / navMeshAgent.speed;
            animationHandler.VelocityX = localVelocity.x;
            animationHandler.VelocityY = localVelocity.z;
        }

        if (!isFalling && ragdollHelper.ragdolled)
        {
            // in transition.
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

    private void UpdateWhenFalling()
    {
        if (!isFallingLastFrame)
        {
            // just released from hand.

        }
        if (rbForSpeedTracking.velocity.magnitude < 0.01)
        {
            // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.WithX(0).WithZ(0));
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            var temp = ragdollHips.transform.parent;
            ragdollHips.transform.parent = null;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.WithX(0).WithZ(0));
            ragdollHips.transform.parent = temp;

            isFalling = false;
            // rb.isKinematic = true;
            //animationHandler.GetComponent<Animator>().applyRootMotion = true;
            rigidbody.isKinematic = true;
            ragdollHelper.ragdolled = false;
            return;
        }
        isFallingLastFrame = isFalling;
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
        // GetComponent<Collider>().enabled = false;
        ragdollHelper.ragdolled = true;

        //ragdollHips.transform.position = GetComponent<CapsuleCollider>().center + transform.position;
        //ragdollHips.transform.rotation = transform.rotation;
        ragdollHips.isKinematic = true;
        if(!navMeshAgent.isStopped)
            navMeshAgent.isStopped = true;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        isBeingHeld = true;
    }

    public void OnHandRelease(Hand hand)
    {
        // GetComponent<Collider>().enabled = true;
        isFalling = true;
        // Disable navigation agent
        //navMeshAgent.isStopped = false;
        //navMeshAgent.updatePosition = true;
        //navMeshAgent.updateRotation = true;
        //navMeshAgent.updateUpAxis = true;
        isBeingHeld = false;

        // Might not be nescessary after sliding after thrown is implemented. Then it would be called after sliding is done.
        // Set navigator current position to current body location.
        // This can be off navigation mesh. This means navigator won't work at all.
        // Will need to be "Warped" again to valid location on nav mesh
        // navMeshAgent.Warp(rb.position);

        // Enable rigidbody physics
        ragdollHips.isKinematic = false;
        rigidbody.isKinematic = false;
        throwable.GetReleaseVelocities(hand, out Vector3 velocity, out Vector3 angularVelocity);

        ragdollHips.velocity = velocity;
        ragdollHips.angularVelocity = angularVelocity;
    }
}
