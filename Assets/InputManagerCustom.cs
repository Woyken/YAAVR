using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerCustom : MonoBehaviour
{
    public Camera mainCamera;
    // Start is called before the first frame update
    void Awake()
    {
    }

    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlayerJoined(PlayerInput playerInput) {
        if(mainCamera.enabled) {
            mainCamera.enabled = false;
        }
        //
        //playerInput.playerIndex
        foreach (var item in playerInput.devices)
        {
            Debug.Log(item.device.deviceId + "   " + item.name);
        }
    }
}
