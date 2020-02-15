using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Valve.VR.InteractionSystem.Throwable))]
/*
This component will be attached to character.
When pickup event is triggered, it will be attached to the hand.
Then character should follow it, while ragdolled.
When detached, reattach itself back to character.
That's one way of preserving character rotation.
*/
public class OnPickupFollowRigidbody : MonoBehaviour
{
    public BotRigidbodyNavigator bot;
    public Valve.VR.InteractionSystem.Throwable throwable;
    public Transform characterRoot;
    public Transform ragdollRoot;
    public Rigidbody ragdollHips;

    new private Rigidbody rigidbody;
    private GameObject myReplacement;
    private bool shouldCharacterFollow = false;

    void Awake()
    {
    }

    private void MoveAllChildrenTo(Transform donor, Transform target)
    {
        for (int i = 0; i < donor.childCount; i++)
        {
            donor.GetChild(i).parent = target;
        }
    }

    void Start()
    {
        throwable.onPickUp.AddListener(() =>
        {
            if (myReplacement == null) {
                myReplacement = new GameObject("PickupTarget replacement");
                myReplacement.transform.parent = characterRoot;
                myReplacement.transform.position = new Vector3();
                myReplacement.transform.rotation = new Quaternion();
            }
                
            MoveAllChildrenTo(transform, myReplacement.transform);
            // Unparent and start following


            //followerRb.freezeRotation = true;
            // followerRb.constraints = RigidbodyConstraints.FreezeAll;
            shouldCharacterFollow = true;
            bot.OnHandPickup();
        });
        throwable.onDetachFromHand.AddListener((hand) =>
        {
            shouldCharacterFollow = false;

            // Gather the velocity with which we are thrown
            throwable.GetReleaseVelocities(throwable.interactable.attachedToHand, out Vector3 velocity, out Vector3 angularVelocity);

            // followerRb.constraints = RigidbodyConstraints.None;
            // followerRb.velocity = followRb.velocity;

            // Reset position and rotation.
            transform.position = new Vector3();
            transform.rotation = new Quaternion();

            // Reparent to character
            transform.parent = characterRoot;

            // Take back my children
            MoveAllChildrenTo(myReplacement.transform, transform);

            // Transfer all released velocity to hips.
            ragdollHips.velocity = velocity;
            ragdollHips.angularVelocity = angularVelocity;
            bot.OnHandRelease(hand);

        });
        //throwable.onHeldUpdate.AddListener((hand) => {
        //followerRb.position = followRb.position;
        //});
    }

    void Update()
    {
        if (shouldCharacterFollow)
        {
            //characterRoot.position = transform.position;
            //ragdollHips.position = transform.position;
            //ragdollHips.rotation = transform.rotation;
        }
    }
}
