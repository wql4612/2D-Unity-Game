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
    public Sprite crackedSprite;
    public Sprite crackedSprite2;

    [HideInInspector] public bool isDestroyed = false;

    private BlockType blockType;
    private float accumulatedDamage = 0f;
    private int crackLevel = 0;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    // 缓存粒子效果
    private GameObject destructionEffect;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        // 根据Tag识别类型
        string tagName = gameObject.tag;
        switch (tagName)
        {
            case "Glass": blockType = BlockType.Glass; break;
            case "Wood": blockType = BlockType.Wood; break;
            case "Stone": blockType = BlockType.Stone; break;
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
                    maxBreakVelocity = 3.1f;
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

        // 初始化对应粒子效果
        destructionEffect = blockType switch
        {
            BlockType.Glass => CreateDestructionEffect("BlueGlassEffect", new Color(0.3f, 0.7f, 1f)),
            BlockType.Wood => CreateDestructionEffect("WoodEffect", new Color(0.55f, 0.27f, 0.07f)),
            BlockType.Stone => CreateDestructionEffect("StoneEffect", new Color(0.5f, 0.5f, 0.5f)),
            _ => null
        };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        Vector2 contactNormal = collision.GetContact(0).normal;
        float impactStrength = Vector2.Dot(playerRb.velocity, -contactNormal);
        impactStrength = Mathf.Abs(impactStrength);

        bool isYellowDash = false;
        BirdSkills skills = collision.gameObject.GetComponent<BirdSkills>();
        if (skills != null && skills.isDashingThrough)
        {
            isYellowDash = true;
            maxBreakVelocity*=0.5f;
        }

        if (isYellowDash && CanFullyBreak(impactStrength))
        {
            FullyDestroy();
            playerRb.velocity = playerRb.velocity * 0.95f;
            return;
        }

        ApplyImpactDamage(impactStrength);

        if (isYellowDash)
        {
            float slowDownFactor = blockType switch
            {
                BlockType.Glass => 0.2f,
                BlockType.Wood => 0.3f,
                BlockType.Stone => 0.8f,
                _ => 0.5f
            };
            playerRb.velocity *= slowDownFactor;
        }
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
                    FullyDestroy();
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
                else if (crackLevel == 1 && accumulatedDamage >= maxBreakVelocity * 0.75f && crackedSprite2 != null)
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

        if (destructionEffect != null)
        {
            GameObject instance = Instantiate(destructionEffect, transform.position, Quaternion.identity);
            Destroy(instance, 2f);
        }

        gameObject.SetActive(false);
    }

    private bool CanFullyBreak(float impact)
    {
        float projectedDamage = accumulatedDamage + impact;

        return blockType switch
        {
            BlockType.Glass => impact >= minBreakVelocity,
            BlockType.Wood => projectedDamage >= maxBreakVelocity,
            BlockType.Stone => projectedDamage >= maxBreakVelocity,
            _ => false
        };
    }

    private GameObject CreateDestructionEffect(string name, Color startColor)
    {
        GameObject go = new GameObject(name);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;

        main.duration = 0.5f;
        main.startLifetime = 0.4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startColor = startColor;
        main.gravityModifier = 0.3f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 50;
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] {
            new ParticleSystem.Burst(0f, 15, 25)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.material.color = startColor;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        return go;
    }
}
