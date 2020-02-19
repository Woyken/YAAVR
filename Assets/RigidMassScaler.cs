using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMassScaler : MonoBehaviour
{
    void Start()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.mass = rb.mass * transform.lossyScale.x;
        }
    }
}
