using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIk : MonoBehaviour
{
    public bool isIKEnabled;
    public Transform rightHandTarget;
    public float rightHandWeight;
    public Transform leftHandTarget;
    public float leftHandWeight;
    public PlayerAnimationHandler animatorProvider;

    void Start()
    {
        animatorProvider.onAnimatorIK += AnimatorIKEvent;
    }

    // called by the Animator Component immediately before it updates its internal IK system
    void AnimatorIKEvent(Animator animator)
    {
        if (!isIKEnabled)
        {
            return;
        }
        if (rightHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetLookAtWeight(0);
        }
    }
}
