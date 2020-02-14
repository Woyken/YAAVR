using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotController : MonoBehaviour
{
    public Transform[] pointsOfInterest;

    public NavMeshAgent navMeshAgent;

    public PlayerAnimationHandler animationHandler;

    public float animationOverdriveMultiplier = 2f;

    [Range(0f, 1f)]
    public float rotationLerp = 0.02f;

    public float maxRotationAngle = 10f;

    public float waitAfterWalkToTargetMin = 0f;

    public float waitAfterWalkToTargetMax = 10f;

    private float restartMovingAt = 0f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationHandler.onAnimatorMoveAction = this.OnAnimatorMove;
    }

    void Start()
    {
        var maxIdx = pointsOfInterest.Length - 1;
        if (maxIdx < 0)
        {
            return;
        }
        var randIdx = Random.Range(0, maxIdx + 1);
        var walkTo = pointsOfInterest[randIdx];
        //navMeshAgent.SetDestination(walkTo.position);
        return;
        // Investigating custom turning with navigation...
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    void Update()
    {
        animationHandler.MovementSpeedMultiplier = navMeshAgent.velocity.magnitude * animationOverdriveMultiplier;
        // animationHandler.VelocityX = inputMove.x * (inputIsSprinting ? 2 : 1);
        var localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);
        localVelocity.y = 0f;
        localVelocity.Normalize();
        animationHandler.VelocityX = localVelocity.x;
        animationHandler.VelocityY = localVelocity.z;


        if (
            (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete
            || navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
        && (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance * 1.2f
        || float.IsInfinity(navMeshAgent.remainingDistance)))
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
            // Investigating custom turning with navigation...
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            return;
        }

        return;
        // Investigating custom turning with navigation...

        //TODO prefer turning before moving. Should add range to set ability to firstly turn to target location, only then start moving. This would be randomized per bot.

        var nextDirection = navMeshAgent.nextPosition - transform.position;
        if(nextDirection.magnitude < 0.0002) {
            navMeshAgent.isStopped = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            //transform.position += nextDirection;
            return;
        }

        var nextRotation = Quaternion.LookRotation(nextDirection.WithY(0));
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, rotationLerp);


        // Attempting to limit turning radius, did not work.
        var angleToTarget = Vector3.Angle(transform.forward, nextDirection);
        if (angleToTarget > maxRotationAngle)
        {
            // This doesn't work properly...
            nextDirection = Vector3.ClampMagnitude(Vector3.Slerp(transform.forward, nextDirection, 1 - (maxRotationAngle / angleToTarget)), nextDirection.magnitude);
            // navMeshAgent.nextPosition += nextDirection;
            //
        }

        transform.position += nextDirection;
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

    void OnAnimatorMove()
    {
        // Update position to agent position
        //transform.position = navMeshAgent.nextPosition;
    }
}
