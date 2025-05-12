using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private BoxCollider2D coll;
    private float dirX = 0f;
    private bool jumpRequested;
    private bool isJumping;
    private enum MovementState { idle, running, jumping, falling };

    //climb ladder
    private float vertical = 0f;
    private bool isLadder;
    private bool isClimbing;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private float lowJumpGravity = 1f;
    [SerializeField] private float fallGravity = 2f;
    [SerializeField] private LayerMask jumpableGround;  
   
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        rb.gravityScale = fallGravity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            if(!jumpRequested)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //×óÓÒÒÆ¶¯
        dirX = Input.GetAxisRaw("Horizontal");
        //ÅÀÌÝ×Ó
        vertical = Input.GetAxisRaw("Vertical");


        //ÌøÔ¾
        if(Input.GetButtonDown("Jump") && (IsGrounded()||isClimbing))
        {
            jumpRequested = true;
        }

        UpdateAnimationState();

        //ÅÀÌÝ×Ó
        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleClimbing();
        ApplyGravity();
    }
    
    private void HandleMovement()
    {
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    private void HandleJump()
    {
        if (jumpRequested)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpSound.Play();

            if (isClimbing)
            {
                isClimbing = false; // ÍÑÀëÅÀÌÝ×´Ì¬
                rb.gravityScale = fallGravity;
            }

            jumpRequested = false;
            isJumping = true;
        }
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, vertical * moveSpeed);
        }
    }

    private void ApplyGravity()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0;
            return;
        }

        if (rb.velocity.y < 0) // ÏÂÂä×´Ì¬
        {
            rb.gravityScale = fallGravity;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) // ¶Ì°´ÌøÔ¾
        {
            rb.gravityScale = fallGravity;
        }
        else // ³¤°´ÌøÔ¾
        {
            rb.gravityScale = lowJumpGravity;
        }
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        if (dirX > 0f)
        {
            //ÅÐ¶ÏidleºÍrun×´Ì¬×ª»»
            state = MovementState.running;
            //·­×ª»­²¼
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

        if(rb.velocity.y > .1f)
        {
            state = MovementState.jumping;  
        }
        else if(rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);

    }
    //ÅÐ¶Ï½ÇÉ«ÊÇ·ñÔÚµØÃæÉÏ£¬¸¨ÖúÌøÔ¾Ìõ¼þÅÐ¶Ï
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}
