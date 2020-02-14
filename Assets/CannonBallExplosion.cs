using UnityEngine;

public class CannonBallExplosion : MonoBehaviour
{
    public float duration = 0.5f;

    private float startTime;
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (startTime + duration < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
