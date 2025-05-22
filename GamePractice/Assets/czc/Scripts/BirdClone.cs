using UnityEngine;

public class BirdClone : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform playerTransform;
    private BirdCloneManager manager;
    private bool isGrounded = false;

    private float fuseDistance = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        manager = FindObjectOfType<BirdCloneManager>();
        manager.RegisterClone(this);
    }

void FixedUpdate()
{
    if (playerTransform == null) return;

    float dist = Vector2.Distance(transform.position, playerTransform.position);

    // 距离足够近就触发融合，无需全部落地
    if (dist < fuseDistance)
    {
        manager.RemoveClone(this);  // 通知 manager
        Destroy(gameObject);
    }
}


    public bool IsGrounded() => isGrounded;
}
