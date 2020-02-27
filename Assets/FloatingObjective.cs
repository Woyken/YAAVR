using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjective : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float xForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float yForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float zForceDirection = 0.0f;
 
    public float speedMultiplier = 1;
 
    void Start()
    {
    }
 
    void Update()
    {
        this.transform.Rotate(xForceDirection * speedMultiplier
                            , yForceDirection * speedMultiplier
                            , zForceDirection * speedMultiplier
                            , Space.Self);
    }
}
