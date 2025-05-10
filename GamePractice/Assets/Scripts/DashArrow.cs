using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashArrow : MonoBehaviour
{
    
    public Rigidbody2D playerRB;
    public Animator arrowAnimator;
    public float dashSpeed = 10f;
    [SerializeField]private bool allowDashing = false;
    [SerializeField]private bool isDashing = false;
    enum ArrowDirection { Up,Down,Left, Right };
    [SerializeField] private ArrowDirection arrowDirection;
    [SerializeField] private string activeAnimParam = "IsActive";
    private float originalGraviyScale; 
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        arrowAnimator = GetComponent<Animator>();
        originalGraviyScale = playerRB.gravityScale;
        SetArrowDirection();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationState();
    }
    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.K) )
        {
            if(isDashing)
            {
                switch (arrowDirection)
                {
                    case ArrowDirection.Up:
                        playerRB.velocity = new Vector2(0, dashSpeed);
                        break;
                    case ArrowDirection.Down:
                        playerRB.velocity = new Vector2(0, -dashSpeed);
                        break;
                    case ArrowDirection.Left:
                        playerRB.velocity = new Vector2(-dashSpeed, 0);
                        break;
                    case ArrowDirection.Right:
                        playerRB.velocity = new Vector2(dashSpeed, 0);
                        break;
                }
                playerRB.gravityScale = 0;
            }
            else if(allowDashing)
            {
                isDashing = true;
            }
        }
        else
        {
            isDashing = false;
            playerRB.gravityScale = originalGraviyScale;
        }
        if(playerRB == null)
        {
            isDashing = false;
            allowDashing = false; 
        }
    }
    private void UpdateAnimationState()
    {
        if(isDashing)
        {
            arrowAnimator.SetBool(activeAnimParam, true);
        }
        else
        {
            arrowAnimator.SetBool(activeAnimParam, false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            allowDashing = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            allowDashing = false;
        }
    }
    private void SetArrowDirection()
    {
        switch (arrowDirection)
        {
            case ArrowDirection.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case ArrowDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case ArrowDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case ArrowDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
        }
    }
}
