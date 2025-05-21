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
    public bool jumpRequested;
    private bool isJumping;
    [SerializeField] private int jumpCount = 2;
    [SerializeField] public bool canJump = true;
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
        //�����ƶ�
        dirX = Input.GetAxisRaw("Horizontal");
        //������
        vertical = Input.GetAxisRaw("Vertical");


        //��Ծ
        if(Input.GetButtonDown("Jump") && (IsGrounded()||isClimbing)&&jumpCount>0)
        {
            jumpCount--;
            jumpRequested = true;
        }
        if(IsGrounded())
        {
            jumpCount = 1;
        }

        UpdateAnimationState();

        //������
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
                isClimbing = false; // ��������״̬
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

        if (rb.velocity.y < 0) // ����״̬
        {
            rb.gravityScale = fallGravity;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) // �̰���Ծ
        {
            rb.gravityScale = fallGravity;
        }
        else // ������Ծ
        {
            if(canJump)
            {
                rb.gravityScale = lowJumpGravity;
            }
            else
            {
                rb.gravityScale = fallGravity; // ������Ծʱǿ��ʹ����������
            }
        }
        if(IsGrounded())
        {
            canJump = true;
        }
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        if (dirX > 0f)
        {
            //�ж�idle��run״̬ת��
            state = MovementState.running;
            //��ת����
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
    //�жϽ�ɫ�Ƿ��ڵ����ϣ�������Ծ�����ж�
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}
