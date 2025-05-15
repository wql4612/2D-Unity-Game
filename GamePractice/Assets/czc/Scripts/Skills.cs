using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSkills : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 记录原始缩放和碰撞体大小
        originalScale = transform.localScale;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            originalColliderSize = box.size;
            originalColliderOffset = box.offset;
        }
    }

    // �����ܣ���ǰ�ٶȼӱ�
    public void ActivateYellowSkill()
    {
        Vector2 currentVelocity = rb.velocity;
        rb.velocity = currentVelocity * 2f;
        Debug.Log("Yellow skill activated: Speed doubled.");
    }

    // �������ܷ���Ԥ��
    public void ActivateBlueSkill()
    {
        Debug.Log("Blue skill activated: Splitting into 3.");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 currentVelocity = rb.velocity;
        if (currentVelocity.magnitude < 0.1f) return;

        float mainScale = 0.5f;
        float cloneScale = 0.5f;

        // 缩小主角
        transform.localScale = originalScale * mainScale;
        AdjustColliderScale(gameObject, mainScale);

        // 计算分裂方向（垂直于当前速度）
        Vector2 perpendicular = new Vector2(-currentVelocity.y, currentVelocity.x).normalized;
        float offset = 1f;

        Vector3 position1 = transform.position + (Vector3)(perpendicular * offset);
        Vector3 position2 = transform.position - (Vector3)(perpendicular * offset);

        // 克隆两个分身
        GameObject clone1 = Instantiate(gameObject, position1, Quaternion.identity);
        GameObject clone2 = Instantiate(gameObject, position2, Quaternion.identity);

        // 设置缩放
        clone1.transform.localScale = originalScale * cloneScale;
        clone2.transform.localScale = originalScale * cloneScale;
        AdjustColliderScale(clone1, cloneScale);
        AdjustColliderScale(clone2, cloneScale);

        // 设置 tag 为 Player
        clone1.tag = "Player";
        clone2.tag = "Player";

        // 防止再分裂
        Destroy(clone1.GetComponent<BirdSkills>());
        Destroy(clone2.GetComponent<BirdSkills>());

        // 添加控制脚本
        clone1.AddComponent<BirdClone>();
        clone2.AddComponent<BirdClone>();


        // 继承速度
        if (clone1.TryGetComponent(out Rigidbody2D rb1)) rb1.velocity = currentVelocity;
        if (clone2.TryGetComponent(out Rigidbody2D rb2)) rb2.velocity = currentVelocity;
        clone1.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        clone2.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
    }


    // 缩放碰撞体大小（注意缩放是乘以缩放因子，不要叠加重复缩放）
    private void AdjustColliderScale(GameObject obj, float scaleFactor)
    {
        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = originalColliderSize * scaleFactor;
            box.offset = originalColliderOffset * scaleFactor;
        }
    }


    [SerializeField] private GameObject explosionPrefab;  // 拖进爆炸动画的 prefab

    public void ActivateBombSkill()
    {
        Debug.Log("Bomb skill activated: Area damage & bounce.");

        // 主体反弹向上
        rb.velocity = new Vector2(rb.velocity.x, 10f);

        // 爆炸特效
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        // 爆炸范围
        float explosionRadius = 2f;
        float explosionForce = 8f;

        // 斜向上的方向（右上）
        Vector2 explosionDirection = new Vector2(1f, 1.5f).normalized;

        // 查找所有爆炸范围内对象
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            // 对 breakable block 造成伤害
            if (hit.TryGetComponent(out BreakableBlock2D block))
            {
                block.ApplyExternalDamage(15f);
            }

            // 如果带刚体则推动
            if (hit.attachedRigidbody != null && hit.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                hit.attachedRigidbody.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
            }
        }
        // 自己向上飞一点（反弹）
        Rigidbody2D myRb = GetComponent<Rigidbody2D>();
        if (myRb != null)
        {
            myRb.velocity = new Vector2(myRb.velocity.x, 10f);
        }
    }


}
