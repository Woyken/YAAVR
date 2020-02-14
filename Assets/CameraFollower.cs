using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        transform.parent = null;
    }

    void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        transform.position = target.position;
    }
}
