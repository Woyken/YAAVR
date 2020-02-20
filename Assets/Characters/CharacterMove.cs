using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    new private Rigidbody rigidbody = null;
    public CharacterController characterController = null;

    public bool canJump = true;
    public bool canRun = true;
    public float moveSpeed = 0.5f;
    public float sprintMultiplier = 2f;
    public float jumpHeight = 1f;

    [HideInInspector]
    public bool inputIsSprinting = false;
    [HideInInspector]
    public float inputJump = 0f;
    //[HideInInspector]
    public Vector2 inputMove = new Vector2(0f, 0f);
    public Vector3 velocity
    {
        get => rigidbody?.velocity ?? characterController.velocity;
    }

    private bool grounded = false;

    void Awake()
    {

    }

    void Start()
    {

    }

    void UpdateForRigidbody()
    {
        var finalMoveSpeed = moveSpeed * (inputIsSprinting ? sprintMultiplier : 1);
        var moveIn = inputMove * finalMoveSpeed;

        var move = transform.right * moveIn.x + transform.forward * moveIn.y;

        // velocity.y += gravity * Time.deltaTime;

        //characterController.Move(velocity * Time.deltaTime);

        float distanceToTheGround = GetComponent<Collider>().bounds.extents.y;
        grounded = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);

        if (grounded && inputJump > 0f && rigidbody.velocity.y < 0.00001)
        {
            move.y += Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y);
        }
        if (move.sqrMagnitude == 0)
        {
            return;
        }
        // Don't loose current falling velocity.
        move.y += rigidbody.velocity.y;

        rigidbody.velocity = move;
    }

    void UpdateForCC()
    {
        var finalMoveSpeed = moveSpeed * (inputIsSprinting ? sprintMultiplier : 1);
        var moveIn = inputMove * finalMoveSpeed;

        var move = transform.right * moveIn.x + transform.forward * moveIn.y;

        move += Physics.gravity;

        if (grounded && inputJump > 0f && characterController.velocity.y < 0.00001)
        {
            move.y += Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y);
        }

        characterController.SimpleMove(move * Time.deltaTime);
        grounded = characterController.isGrounded;
    }

    void Update()
    {
        if (rigidbody != null)
        {
            UpdateForRigidbody();
        }
        else
        {
            UpdateForCC();
        }
    }
}
