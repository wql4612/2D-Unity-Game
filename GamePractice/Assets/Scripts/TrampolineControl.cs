using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineControl : MonoBehaviour
{
    private Animator anim;
    [SerializeField] public float jumpForce = 22f;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if(playerMovement!=null)
            {
                playerMovement.canJump = false;
                Debug.Log("已禁用玩家跳跃权限");
            }
            anim.SetTrigger("TrampolineJump");
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0f);
            playerRb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }
}
