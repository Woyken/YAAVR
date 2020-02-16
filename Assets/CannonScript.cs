using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CannonScript : MonoBehaviour
{
    public GameObject cannonBallPrefab;
    public Transform[] cannonBallPlaces;
    public float respawnTime = 2f;
    public int maxCount = 5;
    public float controlDropDistance = 5f;
    public Transform cannonBallShootPlace;
    public float shootVelocity = 20f;

    private Animator animator;
    private float lastRespawn;
    private int ballSpawnIndex;
    private CannonBall[] cannonBalls;

    PlayerMovementAgent playerControlling = null;

    private GameObject shotBy;

    void Awake()
    {
        animator = GetComponent<Animator>();
        cannonBalls = new CannonBall[maxCount];
    }

    // Start is called before the first frame update
    void Start()
    {
        lastRespawn = Time.time;
        ballSpawnIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RespawnBall();
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (playerControlling != null)
        {
            var distance = Vector3.Distance(playerControlling.transform.position, transform.position);
            if (distance > controlDropDistance)
            {
                playerControlling.OnStopInteracting();
                playerControlling = null;
                transform.rotation = new Quaternion();
                return;
            }
            var head = playerControlling.headPositionAndRotation;
            transform.rotation = head.rotation;
        }
    }

    void RespawnBall()
    {
        if (maxCount > ballSpawnIndex && lastRespawn + respawnTime < Time.time)
        {
            // respawn cannon ball;
            lastRespawn = Time.time;
            Vector3 spawnPosition = cannonBallPlaces[ballSpawnIndex % cannonBallPlaces.Length].position;
            var ballColider = cannonBallPrefab.GetComponent<BoxCollider>();
            var heightOffset = (ballSpawnIndex / cannonBallPlaces.Length) * ballColider.size.y;
            spawnPosition.y += heightOffset;
            var ballGO = Instantiate(cannonBallPrefab, spawnPosition, new Quaternion(), transform);
            cannonBalls[ballSpawnIndex] = ballGO.GetComponent<CannonBall>();
            ballSpawnIndex++;
        }
    }

    public void Interact(PlayerMovementAgent player)
    {
        if (playerControlling != null)
        {
            return;
        }

        playerControlling = player;
    }

    public void InteractingInteract(bool on)
    {
        if (playerControlling != null && on)
        {
            Shoot(playerControlling.gameObject);

        }
    }

    public void Shoot(GameObject by)
    {
        if (ballSpawnIndex < 1) {
            // No more balls for now.
            return;
        }
        animator.SetTrigger("Shoot");
        var shootBall = cannonBalls[ballSpawnIndex - 1];
        // TODO actual shooting;
        shootBall.transform.position = cannonBallShootPlace.position;
        shootBall.OnShoot(transform.forward * shootVelocity);
        shootBall.transform.parent = null;
        //shootBall.SetActive(false);
        ballSpawnIndex--;
    }
}
