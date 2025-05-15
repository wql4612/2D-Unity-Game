using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private BoxCollider2D coll;
    private LineRenderer lineRenderer;
    private BirdSkills birdSkills;

    [Header("Physics Materials")]
    [SerializeField] private PhysicsMaterial2D noFrictionMat;
    [SerializeField] private PhysicsMaterial2D normalMat;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private LayerMask jumpableGround;

    [Header("Slingshot Settings")]
    [SerializeField] private float forceMultiplier = 10f;
    [SerializeField] private float maxDragDistance = 3f;

    [SerializeField] private LayerMask bounceableLayer;

    [Header("Flight Settings")]
    [SerializeField] private float energyLossFactor = 0.8f;
    [SerializeField] private float stopThreshold = 1f;
    private float bounceCooldown = 0.05f;
    private float bounceTimer = 0f;

    private int[] birdInventory = new int[4];

    private Vector2 dragStartPos;
    private float dirX = 0f;
    private bool isAiming = false;
    private bool isInSlingshotFlight = false;

    private enum MovementState { idle, running, jumping, falling }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        birdSkills = GetComponent<BirdSkills>();

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        rb.gravityScale = 2f;

        // 确保碰撞检测模式正确
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        HandleSlingshotInput();

        if (isAiming)
        {
            UpdateAnimationState();
            return;
        }

        if (isInSlingshotFlight)
        {
            HandleRaycastBounce();
            HandleBirdSkills();

            if (rb.velocity.magnitude < stopThreshold)
            {
                ExitSlingshotFlight();
            }

            UpdateAnimationState();
            return;
        }

        HandleMovementInput();
        HandleJump();
        UpdateAnimationState();
    }

    private void HandleMovementInput()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        if (!isInSlingshotFlight)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (Input.GetButton("Jump") && !IsGrounded())
        {
            rb.gravityScale = 1f;
        }
        if (Input.GetButtonUp("Jump") || rb.velocity.y < 0.1f || IsGrounded())
        {
            rb.gravityScale = 2f;
        }
    }

    private void HandleSlingshotInput()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAiming && !isInSlingshotFlight && birdInventory[0] > 0)
        {
            birdInventory[0]--;
            EnterSlingshotMode();
        }

        if (isAiming)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 dragCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 rawForce = dragStartPos - dragCurrentPos;
                Vector2 clampedForce = Vector2.ClampMagnitude(rawForce, maxDragDistance);
                DrawTrajectory(clampedForce * forceMultiplier);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 rawForce = dragStartPos - dragEndPos;
                Vector2 clampedForce = Vector2.ClampMagnitude(rawForce, maxDragDistance);
                ExitSlingshotMode(clampedForce * forceMultiplier);
            }
        }
    }

    private void HandleBirdSkills()
    {
        if (Input.GetKeyDown(KeyCode.K) && birdInventory[1] > 0)
        {
            birdInventory[1]--;
            birdSkills.ActivateYellowSkill();
        }
        if (Input.GetKeyDown(KeyCode.L) && birdInventory[2] > 0)
        {
            birdInventory[2]--;
            birdSkills.ActivateBlueSkill();
        }
        if (Input.GetKeyDown(KeyCode.O) && birdInventory[3] > 0)
        {
            birdInventory[3]--;
            birdSkills.ActivateBombSkill();
        }
    }

    private void EnterSlingshotMode()
    {
        Time.timeScale = 0f;
        isAiming = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    private void ExitSlingshotMode(Vector2 force)
    {
        Time.timeScale = 1f;
        isAiming = false;
        isInSlingshotFlight = true;
        rb.isKinematic = false;

        // 不再禁用碰撞器
        // coll.enabled = false; // ❌ 禁用会导致 raycast 无法检测

        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(-force.x * 0.05f, ForceMode2D.Impulse);
        lineRenderer.positionCount = 0;
    }

    private void ExitSlingshotFlight()
    {
        isInSlingshotFlight = false;
        // coll.enabled = true; // 如果你禁用了要记得恢复（但现在不需要）
        rb.velocity = Vector2.zero;
    }

    private void DrawTrajectory(Vector2 force)
    {
        int pointCount = 20;
        Vector3[] points = new Vector3[pointCount];
        Vector2 velocity = force / rb.mass;
        Vector2 pos = rb.position;
        float timestep = 0.1f;

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(pos.x, pos.y, 0f);
            velocity += Physics2D.gravity * timestep;
            pos += velocity * timestep;
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(points);
    }

    private void HandleRaycastBounce()
    {
        bounceTimer -= Time.deltaTime;
        if (rb.velocity.magnitude < 0.1f || bounceTimer > 0f) return;

        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = rb.Cast(rb.velocity.normalized, hits, rb.velocity.magnitude * Time.deltaTime + 0.1f);

        Debug.DrawRay(transform.position, rb.velocity.normalized * 0.5f, Color.red, 0.2f);
        Debug.Log("Raycast hit count: " + hitCount);

        if (hitCount > 0)
        {
            RaycastHit2D hit = hits[0];

            Vector2 pushOut = hit.normal * 0.05f;
            rb.position = rb.position + pushOut;

            Rigidbody2D otherRb = hit.collider.attachedRigidbody;
            if (otherRb != null && otherRb != rb)
            {
                Vector2 pushDir = rb.velocity.normalized;
                otherRb.AddForce(pushDir * rb.velocity.magnitude * 0.5f, ForceMode2D.Impulse);
                rb.velocity *= 0.5f;
            }
            else
            {
                Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, hit.normal);
                rb.velocity = reflectedVelocity * energyLossFactor;
            }

            bounceTimer = bounceCooldown;
        }
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    public void AddItem(string tag)
    {
        switch (tag)
        {
            case "Cherry": birdInventory[0]++; break;
            case "Banana": birdInventory[1]++; birdInventory[0]++; break;
            case "Kiwi": birdInventory[2]++; birdInventory[0]++; break;
            case "Orange": birdInventory[3]++; birdInventory[0]++; break;
        }
    }
}
