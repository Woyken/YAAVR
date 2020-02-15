using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator animator;

    public Action onAnimatorMoveAction;
 
    /// <summary>
    /// Override speed, multiply animation speed by this value.
    /// Normal speed = 1
    /// </summary>
    public float MovementSpeedMultiplier
    {
        set
        {
            animator.SetFloat("MovementSpeedMultiplier", value);
        }
    }

    /// <summary>
    /// Movement forward/backward speed from -2 to 2.
    /// 0-1 walking, 1-2 running
    /// </summary>
    public float VelocityX
    {
        set
        {
            animator.SetFloat("VelocityX", value);
        }
    }

    /// <summary>
    /// Movement to side speed from -2 to 2.
    /// 0-1 walking, 1-2 running
    /// </summary>
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
