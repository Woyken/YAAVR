using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private Rigidbody rb = null;
    public CharacterController characterController = null;

    public bool canJump = true;
    public bool canRun = true;
    public float moveSpeed = 0.5f;
    public float sprintMultiplier = 2f;
    public float jumpHeight = 1f;
    public float maxJumpDuration = 1f;

    [HideInInspector]
    public bool inputIsSprinting = false;
    [HideInInspector]
    public float inputJump = 0f;
    //[HideInInspector]
    public Vector2 inputMove = new Vector2(0f, 0f);
    public Vector3 velocity
    {
        get => rb?.velocity ?? characterController.velocity;
    }

    private bool grounded = false;
    private bool isJumping = false;
    private float jumpingSince = 0f;

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

        if (grounded && inputJump > 0f && rb.velocity.y < 0.00001)
        {
            move.y += Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y);
        }
        if (move.sqrMagnitude == 0)
        {
            return;
        }
        // Don't loose current falling velocity.
        move.y += rb.velocity.y;

        rb.velocity = move;
    }

    void UpdateForCC()
    {
        var finalMoveSpeed = moveSpeed * (inputIsSprinting ? sprintMultiplier : 1);
        var moveIn = inputMove * finalMoveSpeed;

        var move = transform.right * moveIn.x + transform.forward * moveIn.y;

        move += Physics.gravity / 10;

        if (inputJump > 0f && isJumping && jumpingSince + maxJumpDuration > Time.time)
        {
            var jumpCurrentForce = jumpHeight * (1-((Time.time - jumpingSince) / maxJumpDuration));
            Debug.Log(jumpCurrentForce);
            move.y += (jumpCurrentForce * -1f * Physics.gravity.y);
        } else if (grounded && inputJump > 0f && characterController.velocity.y < 0.00001)
        {
            jumpingSince = Time.time;
            isJumping = true;
            move.y += Mathf.Sqrt(jumpHeight);
        } else {
            isJumping = false;
        }

        characterController.Move(move * Time.deltaTime);
        grounded = characterController.isGrounded;
    }

    void Update()
    {
        if (rb != null)
        {
            UpdateForRigidbody();
        }
        else
        {
            UpdateForCC();
        }
    }
}
