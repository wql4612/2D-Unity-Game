using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWallCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //注意：右墙阻止玩家向右移动
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
            Debug.Log("Player enter right wall collider");
            if (PlayerController.PlayerState.hSpeed >=0f)
            {
                PlayerController.PlayerState.onRightWall = true;
                PlayerController.PlayerState.hSpeed = 0f;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            PlayerController.PlayerState.onRightWall = true;
            if(PlayerController.PlayerState.hSpeed >= 0f)
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
            Debug.Log("Player exit right wall collider");
            PlayerController.PlayerState.onRightWall = false;
        }
    }
}
