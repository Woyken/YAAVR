using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRScaler : MonoBehaviour
{
    private Vector3 playerScaleToSet;
    private bool scaleSet = true;

    private void Awake()
    {
        var scaleForSteamVRInit = new Vector3(1, 1, 1);
        if (transform.localScale != scaleForSteamVRInit)
        {
            // Schedule scale update back to what it's supposed to be.
            UpdatePlayerScaleAsync(transform.localScale);
            // SteamVR will get messed up if scale is not standard.
            transform.localScale = scaleForSteamVRInit;
        }
    }

    private void Update()
    {
        // If I update scale before launching, hands get all messed up and offset.
        // Even Start() is too late for that.
        // Works if updated during first frame. My guess, some initialization that required for scale to be 1x1x1 is run in start of some SteamVR script.
        if (!scaleSet)
        {
            if (transform.localScale != playerScaleToSet)
                transform.localScale = playerScaleToSet;
            scaleSet = true;
        }
    }

    /// <summary>
    /// Do not use this in Awake() and Start(). Make sure SteamVR is initialized before running this.
    /// Will resize VR player rig.
    /// </summary>
    public void UpdatePlayerScale(Vector3 scaleToSet)
    {
        transform.localScale = scaleToSet;
        scaleSet = true;
    }

    /// <summary>
    /// For use in Awake() and Start()
    /// Will schedule and resize VR player rig with next Update() call.
    /// </summary>
    public void UpdatePlayerScaleAsync(Vector3 scaleToSet)
    {
        // Will tell next Update() to resize the scale.
        scaleSet = false;
        playerScaleToSet = scaleToSet;
    }
}
