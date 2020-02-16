using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovementAgent movementAgent;
    public CharacterRagdollThrowable characterThrowable;
    public float animationOverdriveMultiplier = 1f;
    public PlayerAnimationHandler animationHandler;

    void Awake()
    {
        characterThrowable.onRagdollStateChanged += (s) =>
        {
            switch (s)
            {
                case RagdollState.AnimationComplete:
                    {
                        movementAgent.ResetPosition(transform.position);
                        movementAgent.ResetRotation(transform.rotation);
                        movementAgent.isEnabled = true;
                        break;
                    }
                case RagdollState.Ragdoll:
                    {
                        movementAgent.isEnabled = false;
                        break;
                    }
            }
        };
    }

    void Start()
    {
    }

    void Update()
    {
        MoveAnimation();
    }

    void MoveAnimation()
    {
        animationHandler.MovementSpeedMultiplier = movementAgent.GetComponent<Rigidbody>().velocity.WithY(0).magnitude * animationOverdriveMultiplier / (movementAgent.characterMove.inputIsSprinting ? 2 : 1);
        animationHandler.VelocityX = movementAgent.characterMove.inputMove.x * (movementAgent.characterMove.inputIsSprinting ? 2 : 1);
        animationHandler.VelocityY = movementAgent.characterMove.inputMove.y * (movementAgent.characterMove.inputIsSprinting ? 2 : 1);
    }
}
