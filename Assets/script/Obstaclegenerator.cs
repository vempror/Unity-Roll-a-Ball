using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private Transform player;
    [SerializeField] private float distanceBetweenSpawns = 20f;
    [SerializeField] private float spawnAheadDistance = 100f;
    [SerializeField] private int minX = -3;
    [SerializeField] private int maxX = 3;

    private float nextSpawnZ = 0f;

    void Start()
    {
        // spawn a few at the start
        for (int i = 0; i < 5; i++)
        {
            SpawnObstacle();
            nextSpawnZ += distanceBetweenSpawns;
        }
    }

    void Update()
    {
        if (player != null && player.position.z + spawnAheadDistance >= nextSpawnZ)
        {
            SpawnObstacle();
            nextSpawnZ += distanceBetweenSpawns;
        }
    }

    private void SpawnObstacle()
    {
        // pick random prefab
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
        GameObject obstacle = Instantiate(prefab);

        // set position
        float xPos = Random.Range(minX, maxX + 1);
        float yPos = obstacle.transform.localScale.y / 2f;
        obstacle.transform.position = new Vector3(xPos, yPos, nextSpawnZ);

        // ✅ Collider check
        Collider col = obstacle.GetComponent<Collider>();
        if (col == null)
            col = obstacle.AddComponent<BoxCollider>();
        col.isTrigger = false;

        // ✅ Rigidbody check
        Rigidbody rb = obstacle.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obstacle.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; // we manually move it

        // ✅ Add movement behavior
        if (obstacle.GetComponent<ObstacleMoveAndBounce>() == null)
            obstacle.AddComponent<ObstacleMoveAndBounce>();
    }
}


// 👇 New behaviour for each obstacle
public class ObstacleMoveAndBounce : MonoBehaviour
{
    private float moveSpeed = 10f;
    private bool isHit = false;
    private float recoveryTime = 0.6f;

    void Update()
    {
        if (!isHit)
        {
            // Keep moving forward
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isHit)
        {
            isHit = true;
            float dir = Random.value > 0.5f ? 1f : -1f;
            Vector3 push = new Vector3(dir * 5f, 3f, -2f);
            StartCoroutine(MoveAside(push));
        }
    }

    IEnumerator MoveAside(Vector3 push)
    {
        float timer = 0f;
        while (timer < recoveryTime)
        {
            transform.Translate(push * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        // resume forward movement
        isHit = false;
    }
}
