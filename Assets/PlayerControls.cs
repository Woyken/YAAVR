using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAnimationHandler))]
public class PlayerControls : MonoBehaviour
{
    public Camera playerCamera;
    public Transform cameraPlaceholder;
    public PlayerOverlay overlay;

    public CharacterLook characterLook;
    public CharacterMove characterMove;

    public float animationOverdriveMultiplier = 1f;


    // public float gravity = -9.81f;



    [HideInInspector]
    public Transform headPositionAndRotation { get => playerCamera.transform; }
    [HideInInspector]
    public CannonScript currentlyInteracting = null;

    private PlayerAnimationHandler animationHandler;
    new private Rigidbody rigidbody;

    private float inputInteract = 0f;


    public void OnMove(UnityEngine.InputSystem.InputValue value)
    {
        var inputMove = value.Get<Vector2>();
        characterMove.inputMove = inputMove;
        //Debug.Log(inputMove);
    }

    public void OnLook(UnityEngine.InputSystem.InputValue value)
    {
        var inputLook = value.Get<Vector2>();
        characterLook.inputLook = inputLook;
    }

    public void OnSprint(UnityEngine.InputSystem.InputValue value)
    {
        var inputIsSprinting = value.Get<float>() > 0;
        characterMove.inputIsSprinting = inputIsSprinting;
    }

    public void OnInteract(UnityEngine.InputSystem.InputValue value)
    {
        if (currentlyInteracting != null)
        {
            currentlyInteracting.InteractingInteract(value.Get<float>() > 0);
            return;
        }
        inputInteract = value.Get<float>();
    }

    public void OnJump(UnityEngine.InputSystem.InputValue value)
    {
        var inputJump = value.Get<float>();
        characterMove.inputJump = inputJump;
        //Debug.Log(inputJump);
    }

    public void OnStopInteracting()
    {
        currentlyInteracting = null;
    }

    void Awake()
    {
        animationHandler = GetComponent<PlayerAnimationHandler>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        //Look();
    }

    void FixedUpdate()
    {
        LayerMask interactableLayer = LayerMask.GetMask("Interactable");
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit interactionInfo, 20f, interactableLayer))
        {
            // Something interactable is in front.
            GameObject interactedObject = interactionInfo.collider.gameObject;
            if (!overlay.gameObject.activeSelf)
            {
                overlay.gameObject.SetActive(true);
            }
            if (inputInteract > 0)
            {
                inputInteract = 0f;
                var interactable = interactedObject.GetComponentInParent<CannonScript>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                    currentlyInteracting = interactable;
                    // TODO add some animations
                }
            }
        }
        else if (overlay.gameObject.activeSelf)
        {
            overlay.gameObject.SetActive(false);
        }
    }

    void Move()
    {
        

        animationHandler.MovementSpeedMultiplier = rigidbody.velocity.magnitude * animationOverdriveMultiplier / (characterMove.inputIsSprinting ? 2 : 1);
        animationHandler.VelocityX = characterMove.inputMove.x * (characterMove.inputIsSprinting ? 2 : 1);
        animationHandler.VelocityY = characterMove.inputMove.y * (characterMove.inputIsSprinting ? 2 : 1);
    }
}
