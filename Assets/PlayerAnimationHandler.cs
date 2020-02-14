using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator animator;

    public Action onAnimatorMoveAction;
 
    public float MovementSpeedMultiplier
    {
        set
        {
            animator.SetFloat("MovementSpeedMultiplier", value);
        }
    }

    public float VelocityX
    {
        set
        {
            animator.SetFloat("VelocityX", value);
        }
    }

    public float VelocityY
    {
        set
        {
            animator.SetFloat("VelocityY", value);
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        onAnimatorMoveAction?.Invoke();
    }
}
