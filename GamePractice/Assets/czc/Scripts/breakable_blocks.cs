using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 可破坏的方块：受玩家撞击后可进入部分破坏状态，连续撞击会完全破坏
/// </summary>
public class BreakableBlock2D : MonoBehaviour
{
    [Header("破坏参数")]
    [SerializeField] public float minBreakVelocity = 5f;     // 撞击大于此速度将部分破坏
    [SerializeField] public float maxBreakVelocity = 10f;    // 累计破坏值超过此值会完全破坏

    [Header("状态贴图/动画变量（可以接入动画系统）")]
    public Sprite crackedSprite;           // 替换精灵（部分破坏时）
    public bool isDestroyed = false;

    [Header("破坏反馈")]
    public GameObject destroyEffectPrefab;  // 完全破坏时的粒子/特效

    private float accumulatedDamage = 0f;   // 累计的撞击值
    private bool isCracked = false;

    private SpriteRenderer spriteRenderer;  // 精灵渲染器
    private Rigidbody2D rb;  // 刚体

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 确保方块可物理受力（非Kinematic）
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        // 获取撞击点的法线方向，和玩家速度方向的投影值
        Vector2 contactNormal = collision.GetContact(0).normal;
        float impactStrength = Vector2.Dot(playerRb.velocity, -contactNormal); // 近似撞击速度

        // 取绝对值，避免负数
        impactStrength = Mathf.Abs(impactStrength);
        accumulatedDamage += impactStrength;

        if (!isCracked && crackedSprite != null&&accumulatedDamage>=minBreakVelocity)
        {
            EnterCrackedState();
        }

        if (accumulatedDamage >= maxBreakVelocity)
        {
            FullyDestroy();
        }

    }

    private void EnterCrackedState()
    {
        isCracked = true;
        spriteRenderer.sprite = crackedSprite; // 替换为破碎贴图
        // 也可以在这里触发动画，例如：animator.SetTrigger("Crack");
    }

    private void FullyDestroy()
    {
        isDestroyed = true;

        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }
        // 这里你可以接动画系统，也可以只设置为不可见：
        gameObject.SetActive(false); // 或 Destroy(gameObject);
    }
}
