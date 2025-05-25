using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public enum InitialDirection
    {
        Up,
        Down,
        Left,
        Right,
        Custom
    }

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // 固定速度大小
    [SerializeField] private float lifeTime = 10f; // 存在时间
    [Header("Velocity Settings")]
    [SerializeField] private bool useRandomVelocity = true;
    [SerializeField] private InitialDirection initialDirection = InitialDirection.Up;
    [SerializeField] private Vector2 customDirection = Vector2.right;

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
        if (useRandomVelocity)
        {
            SetRandomDirection();
        }
        else
        {
            SetInitialDirection();
        }
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

    // 设置指定方向
    private void SetInitialDirection()
    {
        switch (initialDirection)
        {
            case InitialDirection.Up:
                currentDirection = Vector2.up;
                break;
            case InitialDirection.Down:
                currentDirection = Vector2.down;
                break;
            case InitialDirection.Left:
                currentDirection = Vector2.left;
                break;
            case InitialDirection.Right:
                currentDirection = Vector2.right;
                break;
            case InitialDirection.Custom:
                currentDirection = customDirection.normalized;
                break;
        }
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
}
