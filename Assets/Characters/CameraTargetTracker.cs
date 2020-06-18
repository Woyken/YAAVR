using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetTracker : MonoBehaviour
{
    new Camera camera;
    public float aimAssistRadius = 2f;

    private Vector3 lastHitPoint = new Vector3(100, 100, 100);

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void FixedUpdate()
    {
        // TODO, optimize, we probably don't need to call this every update
        var ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // For now ignore all players. Ideally should ignore only self.
        // Potential solution - could reserve 4 layers for player. PlayerManager would be responsible to provide each one with layer id.
        var layerMask = ~LayerMask.GetMask("Player");

        if (Physics.SphereCast(ray, aimAssistRadius, out var hit, 20f, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            var receiveDamage = hit.transform.GetComponent<ReceiveDamage>();
            if (receiveDamage != null)
            {
                // Correct target.
                lastHitPoint = hit.point;
            }
            // Ignore walls
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastHitPoint, 0.1f);
    }
}
