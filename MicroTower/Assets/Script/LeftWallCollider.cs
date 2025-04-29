using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //注意：左墙阻止角色向左移动
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player enter left wall collider");
            if (PlayerController.PlayerState.hSpeed <=0f)
            {
                PlayerController.PlayerState.onLeftWall = true;
                PlayerController.PlayerState.hSpeed = 0f;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if(player != null)
        {
            PlayerController.PlayerState.onLeftWall = true;
            if(PlayerController.PlayerState.hSpeed <= 0f)
            {   
                PlayerController.PlayerState.hSpeed = 0f;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player exit left wall collider");
            PlayerController.PlayerState.onLeftWall = false;
        }
    }
}
