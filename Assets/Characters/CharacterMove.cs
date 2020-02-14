using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    new public Rigidbody rigidbody;

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

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        var finalMoveSpeed = moveSpeed * (inputIsSprinting ? sprintMultiplier : 1);
        var moveIn = inputMove * finalMoveSpeed;

        var move = transform.right * moveIn.x + transform.forward * moveIn.y;

        // velocity.y += gravity * Time.deltaTime;

        //characterController.Move(velocity * Time.deltaTime);

        float distanceToTheGround = GetComponent<Collider>().bounds.extents.y;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);

        if (isGrounded && inputJump > 0f && rigidbody.velocity.y < 0.00001)
        {
            move.y += Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y);
        }
        if(move.sqrMagnitude == 0) {
            return;
        }
        // Don't loose current falling velocity.
        move.y += rigidbody.velocity.y;

        rigidbody.velocity = move;
    }
}
