using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// I really want to use ragdoll physics for player movement
// But there's a problem now wit hall the ragdoll additions
// I can't have big enough collider for whole player, the grabbing distance won't be the same as bot.
// Also the rigidbody must be enabled when ragdolling.
// So if I make it a big collider for general player movement it will interfere with ragdoll physics.
// Maybe there's better solution, but I'm making this into player navigation agent.
// It will act similarly to navmeshagent.
// But instead of purely internal position calculations, I'll have different GameObject for player movements, and the body will just follow it
// Having it as parent object and detaching on start at the moment seems like a viable solution
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementAgent : MonoBehaviour
{
    public Camera playerCamera;
    public PlayerOverlay overlay;
    public Transform playerRoot;

    public CharacterLook characterLook;
    public CharacterMove characterMove;

    public bool shouldControlMovePlayer = true;
    public bool shouldControlTurnCamera
    {
        get { return characterLook.isEnabled; }
        set { characterLook.isEnabled = value; }
    }
    public bool isEnabled = true;



    [HideInInspector]
    public Transform headPositionAndRotation { get => playerCamera.transform; }
    [HideInInspector]
    public CannonScript currentlyInteracting = null;

    new private Rigidbody rigidbody;

    private float inputInteract = 0f;


    public void OnMove(UnityEngine.InputSystem.InputValue value)
    {
        if (!isEnabled)
        {
            characterMove.inputMove = new Vector2();
            return;
        }
        var inputMove = value.Get<Vector2>();
        characterMove.inputMove = inputMove;
        //Debug.Log(inputMove);
    }

    public void OnLook(UnityEngine.InputSystem.InputValue value)
    {
        if (!isEnabled)
        {
            characterLook.inputLook = new Vector2();
            return;
        }
        var inputLook = value.Get<Vector2>();
        characterLook.inputLook = inputLook;
    }

    public void OnSprint(UnityEngine.InputSystem.InputValue value)
    {
        if (!isEnabled)
        {
            characterMove.inputIsSprinting = false;
            return;
        }
        var inputIsSprinting = value.Get<float>() > 0;
        characterMove.inputIsSprinting = inputIsSprinting;
    }

    public void OnInteract(UnityEngine.InputSystem.InputValue value)
    {
        if (!isEnabled)
        {
            inputInteract = 0f;
            return;
        }
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

    public void ResetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ResetRotation(Quaternion q)
    {
        transform.rotation = q;
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = 0;

        Collider collider1 = GetComponent<Collider>();
        if (collider1 != null)
        {
            foreach (Collider child in playerRoot.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(collider1, child);
            }
        }
    }

    void Start()
    {
        transform.parent = null;
    }

    void Update()
    {
        if (!isEnabled)
        {
            return;
        }
        if (shouldControlMovePlayer)
        {
            playerRoot.position = transform.position;
        }
        if (shouldControlTurnCamera)
        {
            playerRoot.rotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!isEnabled)
        {
            return;
        }
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
}
