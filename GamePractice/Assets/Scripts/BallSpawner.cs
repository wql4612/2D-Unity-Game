using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private Vector2 spawnAreaCenter;
    [SerializeField] private Vector2 spawnAreaSize;

    [Header("Ball Speed Settings")]
    [SerializeField] private float maxSpeed = 5f;

    private float timer;
    private bool canSpawn = false;
    private List<GameObject> spawnedBalls = new List<GameObject>();

    private void Start()
    {
        // 初始化生成区域中心和大小
        spawnAreaCenter = transform.position;
        spawnAreaSize = new Vector2(5f, 5f); // 可以根据需要调整
    }
    private void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBallAtRandomPosition();
            timer = 0f;
        }
    }

    private void SpawnBallAtRandomPosition()
    {
        Vector2 spawnPos = new Vector2(
            spawnAreaCenter.x + Random.Range(-spawnAreaSize.x/2, spawnAreaSize.x/2),
            spawnAreaCenter.y + Random.Range(-spawnAreaSize.y/2, spawnAreaSize.y/2)
        );

        GameObject ball = ObjectPool.Instance.GetFromPool("Ball", spawnPos, Quaternion.identity);
        if (ball == null)//先检测对象池是否已满
        {
            Debug.LogWarning("Ball pool is empty or reached max size, cannot spawn new ball.");
            return;
        }
        spawnedBalls.Add(ball);

        // 给球一个随机速度
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = Random.Range(0f, maxSpeed);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            rb.velocity = velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSpawn = true;
            timer = 0f; // 立即开始计时
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSpawn = false;
            timer = 0f;
            // 销毁所有生成的球体
            foreach (var ball in spawnedBalls)
            {
                if (ball.activeInHierarchy)
                {
                    ObjectPool.Instance.ReturnToPool("Ball", ball);
                }
            }
            spawnedBalls.Clear();
        }
    }

    // 在Scene视图中显示生成区域
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
