using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWallCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player stay on right wall collider");
            if (PlayerController.PlayerState.vSpeed >0f)
            {
                PlayerController.PlayerState.faceLeft = true;
                PlayerController.PlayerState.onWall = true;
                PlayerController.PlayerState.vSpeed = 0f;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player exit right wall collider");
            PlayerController.PlayerState.onWall = false;
        }
    }
}
