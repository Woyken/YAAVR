using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class CharacterLook : MonoBehaviour
{
    //public Vector2 
    [Tooltip("Camera to controll, can be left empty if not needed.")]
    new public Camera camera;
    public Transform body;
    public float sensitivity = 10f;
    public bool isEnabled = true;

    [HideInInspector]
    public Vector2 inputLook = new Vector2(0f, 0f);

    private float yRotation = 0f;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        if (!isEnabled)
        {
            return;
        }
        var targetDirection = camera.transform.forward.WithY(0);
        body.rotation = Quaternion.LookRotation(targetDirection);
    }
}
