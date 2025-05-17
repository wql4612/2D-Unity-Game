using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
   [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // 固定速度大小
    [SerializeField] private float lifeTime = 10f; // 存在时间

    private Rigidbody2D rb;
    private float activeTime;
    private Vector2 currentDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 对象被取出时的初始化
    public void OnSpawn()
    {
        activeTime = 0f;
        SetRandomDirection();
        ApplyVelocity();
        StartCoroutine(AutoRecycleCoroutine());
    }

    // 对象被回收时的清理
    public void OnDespawn()
    {
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
    }

    // 设置随机方向（单位向量）
    private void SetRandomDirection()
    {
        float angle = Random.Range(0, 360f);
        currentDirection = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;
    }

    // 应用速度
    private void ApplyVelocity()
    {
        rb.velocity = currentDirection * speed;
    }

    // 自动回收协程
    private System.Collections.IEnumerator AutoRecycleCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPool.Instance.ReturnToPool("Ball", gameObject);
    }

    // 碰撞处理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Ground"))
        // {
        //     // 使用物理引擎的反射计算
        //     Vector2 normal = collision.contacts[0].normal;
        //     currentDirection = Vector2.Reflect(currentDirection, normal).normalized;
        //     ApplyVelocity();
        // }
    }
}
