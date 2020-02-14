using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CannonBall : MonoBehaviour
{
    public GameObject explosionPrefab;

    new private Rigidbody rigidbody;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void OnShoot(Vector3 velocity)
    {
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.velocity = velocity / rigidbody.mass;
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        // Rotate the object so that the y-axis faces along the normal of the surface
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(explosionPrefab, pos, rot);
        Destroy(gameObject);
    }
}
