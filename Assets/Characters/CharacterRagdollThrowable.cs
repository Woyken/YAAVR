using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum RagdollState
{
    Ragdoll,
    ManualTransition,
    StandUpAnimation,
    AnimationComplete,
}

public class CharacterRagdollThrowable : MonoBehaviour
{
    public delegate void RagdollStateChangeEventHandler(RagdollState state);
    public event RagdollStateChangeEventHandler onRagdollStateChanged;

    public Throwable throwable;
    public Rigidbody ragdollHipsRb;
    public Transform ragdollHips;
    public Animator animator;
    public RagdollHelper ragdollHelper;
    [HideInInspector]
    public RagdollState state = RagdollState.StandUpAnimation;
    public float ragdollLayOnGroundFor = 2f;

    private new Rigidbody rigidbody;
    private float canDisableRagdollAt;
    private bool isPickedUp = false;
    private bool isStandingUpAnimation = false;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        throwable.onDetachFromHand.AddListener(OnHandRelease);
        throwable.onPickUp.AddListener(OnHandPickup);
        ragdollHelper.onStateChanged += (s) =>
        {
            switch (s)
            {
                case RagdollHelper.RagdollState.animated:
                    state = RagdollState.StandUpAnimation;
                    onRagdollStateChanged?.Invoke(state);
                    break;
                case RagdollHelper.RagdollState.blendToAnim:
                    state = RagdollState.ManualTransition;
                    onRagdollStateChanged?.Invoke(state);
                    break;
                case RagdollHelper.RagdollState.ragdolled:
                    state = RagdollState.Ragdoll;
                    onRagdollStateChanged?.Invoke(state);
                    break;
            }
        };

        // Disable collisions for all child colliders.
        // We have to have collider on parent as well as for the whole ragdoll.
        // Otherwise ragdoll will fall "away" from parent element.
        // Recovering from that is really hard, we would have to figure out base location from the ragdoll.
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
        ragdollHips.gameObject.SetActive(false);
    }

    void Update()
    {
        if (this.state == RagdollState.StandUpAnimation)
        {
            // Another dirty hack.
            // Detecting when standing up animation is complete. 
            // Should probably use regular animation events. Although I can't be sure that it will be sent for sure.
            var animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (animationName != "stand_up_from_back_3" && animationName != "standing_up_from_belly_2")
            {
                ragdollHips.gameObject.SetActive(false);
                this.state = RagdollState.AnimationComplete;
                onRagdollStateChanged?.Invoke(state);
            }
        }

        HandleRagdollDisabling();
    }

    void HandleRagdollDisabling()
    {
        if (!isPickedUp && state == RagdollState.Ragdoll)
        {
            if (ragdollHipsRb.velocity.magnitude >= 0.05)
            {
                canDisableRagdollAt = Time.time + ragdollLayOnGroundFor;
            }
            else
            {
                if (canDisableRagdollAt > Time.time)
                {
                    return;
                }
                // Hack. We need to reset the rotation of character. After released from hand, might not be facing up.
                // Detach Hips, rotate parent, reatach hips.
                // This way we fix rotation ofthe whole object without moving the ragdoll itself.
                if (ragdollHips.parent != null)
                {
                    var originalParent = ragdollHips.parent;
                    ragdollHips.parent = null;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.WithX(0).WithZ(0));
                    ragdollHips.parent = originalParent;
                }

                rigidbody.isKinematic = true;
                ragdollHelper.ragdolled = false;
                return;
            }
        }
    }

    public void OnHandPickup()
    {
        ragdollHips.gameObject.SetActive(true);

        ragdollHelper.ragdolled = true;
        ragdollHipsRb.isKinematic = true;
        isPickedUp = true;
    }

    public void OnHandRelease(Hand hand)
    {
        // Might not be nescessary after sliding after thrown is implemented. Then it would be called after sliding is done.
        // Set navigator current position to current body location.
        // This can be off navigation mesh. This means navigator won't work at all.
        // Will need to be "Warped" again to valid location on nav mesh
        // navMeshAgent.Warp(rb.position);

        // Enable rigidbody physics
        ragdollHipsRb.isKinematic = false;
        rigidbody.isKinematic = false;
        throwable.GetReleaseVelocities(hand, out Vector3 velocity, out Vector3 angularVelocity);

        ragdollHipsRb.velocity = velocity;
        ragdollHipsRb.angularVelocity = angularVelocity;
        isPickedUp = false;
    }
}
