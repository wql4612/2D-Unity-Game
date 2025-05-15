using UnityEngine;

/// <summary>
/// 可破坏方块：支持玻璃、木头、石头三种材质，自动根据Tag识别
/// </summary>
public class BreakableBlock2D : MonoBehaviour
{
    public enum BlockType { Glass, Wood, Stone }

    [Header("是否启用自定义破坏参数")]
    public bool overrideDefaults = false;

    [Header("破坏参数（可选手动覆盖）")]
    public float minBreakVelocity = 5f;
    public float maxBreakVelocity = 10f;

    [Header("状态贴图")]
    public Sprite crackedSprite;    // 木头、石头：第一阶段
    public Sprite crackedSprite2;   // 石头：第二阶段

    [Header("破坏反馈")]
    public GameObject destroyEffectPrefab;

    [HideInInspector] public bool isDestroyed = false;

    private BlockType blockType;
    private float accumulatedDamage = 0f;
    private int crackLevel = 0;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

  void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        // 根据Tag识别类型
        string tagName = gameObject.tag;
        switch (tagName)
        {
            case "Glass":
                blockType = BlockType.Glass;
                break;
            case "Wood":
                blockType = BlockType.Wood;
                break;
            case "Stone":
                blockType = BlockType.Stone;
                break;
            default:
                Debug.LogWarning($"未知材质标签：{tagName}，默认按石头处理");
                blockType = BlockType.Stone;
                break;
        }

        // 自动设置默认破坏参数（如果未手动覆盖）
        if (!overrideDefaults)
        {
            switch (blockType)
            {
                case BlockType.Glass:
                    minBreakVelocity = 3f;
                    maxBreakVelocity = 3.1f; // 其实直接摧毁用不到 max
                    break;
                case BlockType.Wood:
                    minBreakVelocity = 5f;
                    maxBreakVelocity = 10f;
                    break;
                case BlockType.Stone:
                    minBreakVelocity = 8f;
                    maxBreakVelocity = 15f;
                    break;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        Vector2 contactNormal = collision.GetContact(0).normal;
        float impactStrength = Vector2.Dot(playerRb.velocity, -contactNormal);
        impactStrength = Mathf.Abs(impactStrength);

        ApplyImpactDamage(impactStrength);
    }

    public void ApplyExternalDamage(float damage)
    {
        ApplyImpactDamage(damage);
    }

    private void ApplyImpactDamage(float impact)
    {
        accumulatedDamage += impact;

        switch (blockType)
        {
            case BlockType.Glass:
                if (impact >= minBreakVelocity)
                {
                    FullyDestroy();
                }
                break;

            case BlockType.Wood:
                if (crackLevel == 0 && accumulatedDamage >= minBreakVelocity && crackedSprite != null)
                {
                    spriteRenderer.sprite = crackedSprite;
                    crackLevel = 1;
                }
                else if (accumulatedDamage >= maxBreakVelocity)
                {
                    FullyDestroy();
                }
                break;

            case BlockType.Stone:
                if (crackLevel == 0 && accumulatedDamage >= minBreakVelocity && crackedSprite != null)
                {
                    spriteRenderer.sprite = crackedSprite;
                    crackLevel = 1;
                }
                else if (crackLevel == 1 && accumulatedDamage >= (maxBreakVelocity * 0.75f) && crackedSprite2 != null)
                {
                    spriteRenderer.sprite = crackedSprite2;
                    crackLevel = 2;
                }
                else if (accumulatedDamage >= maxBreakVelocity)
                {
                    FullyDestroy();
                }
                break;
        }
    }

    private void FullyDestroy()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); // 或 Destroy(gameObject);
    }
}
