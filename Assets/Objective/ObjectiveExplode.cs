using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

enum Status
{
    Normal,
    Unstable,
    Exploded,
}

public class ObjectiveExplode : MonoBehaviour
{
    public float timeBeforeExplosion = 2f;
    public VisualEffect normal;
    public VisualEffect unstable;
    public VisualEffect explosion;
    public GameObject brokenVersion;
    public GameObject originalObject;

    private float activatedAt = 0f;

    private Status status = Status.Normal;
    void Start()
    {
        normal.Play();
        unstable.Stop();
        explosion.Stop();
    }

    void Update()
    {
        switch (status)
        {
            case Status.Unstable:
                {
                    if (activatedAt + timeBeforeExplosion < Time.time)
                    {
                        // Boob boom time
                        unstable.Stop();
                        explosion.Play();
                        status = Status.Exploded;
                    }
                    break;
                }
            default:
                break;
        }
    }

    public void Activate()
    {
        if (status == Status.Normal)
        {
            GetComponent<Collider>().enabled = false;
            normal.Stop();
            unstable.Play();
            status = Status.Unstable;
            activatedAt = Time.time;
        }
    }
}
