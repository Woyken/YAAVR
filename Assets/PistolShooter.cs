using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class PistolShooter : MonoBehaviour
{
    public GameObject bulletPreset;
    public Transform bulletSpawnPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 targetLocation)
    {
        var direction = targetLocation - bulletSpawnPoint.position;
        direction.Normalize();
        var bullet = Instantiate(bulletPreset, bulletSpawnPoint.position, Quaternion.LookRotation(direction));
        // Stupidly set cannon ball direction for now. TODO.
        (bullet.GetComponentInChildren<CannonBall>()).OnShoot(direction / 10000);
        bullet.GetComponentInChildren<Rigidbody>().useGravity = false;
    }
}
