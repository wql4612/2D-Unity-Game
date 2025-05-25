using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SpikeHeadCtrl : MonoBehaviour
{
    public enum HitDirection { Left, Right, Down }
    [Header("Movement Settings")]
    public HitDirection hitDirection = HitDirection.Down;
    public float acceleration = 20f;
    public float maxSpeed = 10f;
    public float returnSpeed = 5f;
    public float pauseTime = 0.5f;
    public bool isAuto = true; // 是否自动撞击

    private Vector2 initialPosition;
    private Vector2 moveDir;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private bool isReturning = false;
    private bool isPaused = false;
    private bool canAttack = false;

    // 动画相关
    private Animator animator;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int LeftHit = Animator.StringToHash("LeftHit");
    private static readonly int RightHit = Animator.StringToHash("RightHit");
    private static readonly int BottomHit = Animator.StringToHash("BottomHit");

    private Collider2D triggerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        moveDir = GetDirection();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.freezeRotation = true; // 锁定旋转
        PlayIdle();

        // 获取Trigger子物体的Collider2D
        triggerCollider = transform.Find("Trigger")?.GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    void FixedUpdate()
    {
        if (isPaused) return;

        if (!isReturning)
        {
            if (isAuto || (!isAuto && canAttack) || isMoving)
            {
                isMoving = true;//保证如果运动开始只会被撞击中断
                // 加速向指定方向运动
                rb.velocity += moveDir * acceleration * Time.fixedDeltaTime;
                if (rb.velocity.magnitude > maxSpeed)
                    rb.velocity = moveDir * maxSpeed;

                // 播放Move动画
                PlayMove();
            }
        }
        else
        {
            // 匀速返回
            Vector2 toInit = initialPosition - (Vector2)transform.position;
            if (toInit.magnitude < 0.05f)
            {
                rb.velocity = Vector2.zero;
                transform.position = initialPosition;
                isReturning = false;
                PlayIdle();
                // 重新开始撞击
            }
            else
            {
                Vector2 returnDir = toInit.normalized;
                rb.velocity = returnDir * returnSpeed;
                // 返回时播放Idle动画
                PlayIdle();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isReturning)
        {
            // 撞击到场景，暂停
            isMoving = false;
            PlayHit();
            StartCoroutine(PauseAndReturn());
        }
        else
        {
            // 返回途中被阻挡
            rb.velocity = Vector2.zero;
            initialPosition = transform.position;
            isReturning = false;
            PlayIdle();
        }
    }

    private IEnumerator PauseAndReturn()
    {
        isPaused = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(pauseTime);
        isPaused = false;
        isReturning = true;
    }

    private Vector2 GetDirection()
    {
        switch (hitDirection)
        {
            case HitDirection.Left: return Vector2.left;
            case HitDirection.Right: return Vector2.right;
            case HitDirection.Down: return Vector2.down;
            default: return Vector2.down;
        }
    }

    // 动画控制方法
    private void PlayIdle()
    {
        if (animator != null)
            animator.Play(Idle);
    }
    private void PlayMove()
    {
        if (animator != null)
            animator.Play(Move);
    }
    private void PlayHit()
    {
        if (animator == null) return;
        switch (hitDirection)
        {
            case HitDirection.Left:
                animator.Play(LeftHit);
                break;
            case HitDirection.Right:
                animator.Play(RightHit);
                break;
            case HitDirection.Down:
                animator.Play(BottomHit);
                break;
            default:
                animator.Play(BottomHit);
                break;
        }
    }
    // 只在手动模式下响应Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAuto && other.CompareTag("Player"))
        {
            canAttack = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isAuto && other.CompareTag("Player"))
        {
            canAttack = false;
        }
    }
}
