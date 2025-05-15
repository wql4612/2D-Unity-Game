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

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist < fuseDistance && manager.AllClonesGrounded())
        {
            manager.FuseAllClones();
        }
    }

    public bool IsGrounded() => isGrounded;
}
