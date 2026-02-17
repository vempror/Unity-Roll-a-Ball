using UnityEngine;

public class PoolReturner : MonoBehaviour
{
    private ObjectPool pool;
    private Transform player;

    void Start()
    {
        GameObject poolObj = GameObject.Find("ObstaclePool");
        if (poolObj != null)
        {
            pool = poolObj.GetComponent<ObjectPool>();
        }
        else
        {
            Debug.LogError("❌ ObstaclePool not found in scene!");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("❌ Player not found or not tagged 'Player'!");
        }
    }

    void Update()
    {
        if (player != null && pool != null && transform.position.z < player.position.z - 30f)
        {
            pool.ReturnObject(gameObject);
        }
    }
}
