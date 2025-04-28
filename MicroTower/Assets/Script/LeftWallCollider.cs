using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
            if (PlayerController.PlayerState.hSpeed >=0f)
            {
                PlayerController.PlayerState.faceLeft = false;
                PlayerController.PlayerState.onWall = true;
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
            PlayerController.PlayerState.onWall = false;
        }
    }
}
