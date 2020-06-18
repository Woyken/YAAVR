using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRScaler : MonoBehaviour
{
    [SerializeField] Vector3 playerScale = new Vector3(1, 1, 1);

    private bool scaleSet = false;

    void Update()
    {
        // If I update scale before launching, hands get all messed up and offset.
        // Even Start() is too late for that.
        // Works if updated during first frame. My guess, some initialization that required for scale to be 1x1x1 is run in start of some SteamVR script.
        if (!scaleSet)
        {
            transform.localScale = playerScale;
            scaleSet = true;
            // Even destroy this script so it doesn't get ticket anymore.
            Destroy(this);
        }
    }
}
