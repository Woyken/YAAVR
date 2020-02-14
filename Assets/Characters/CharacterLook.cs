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
    public float sensitivity = 100f;

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
        var lookDiff = inputLook * sensitivity * Time.deltaTime;

        yRotation -= lookDiff.y;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        if (Mathf.Abs(lookDiff.x) > 0)
        {
            body.Rotate(Vector3.up, lookDiff.x);
        }

        if (camera == null)
        {
            return;
        }

        if (camera.transform.parent == null)
        {
            camera.transform.rotation = body.rotation;
            camera.transform.eulerAngles = camera.transform.eulerAngles.WithX(yRotation);
        }
        else
        {
            camera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        }
    }
}
