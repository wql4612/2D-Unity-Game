using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollisper : MonoBehaviour
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
            Debug.Log("Player enter floor collider");
            PlayerController.PlayerState.inAir = false;
            PlayerController.PlayerState.vSpeed = 0f;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player stay in floor collider");
            PlayerController.PlayerState.inAir = false;
            PlayerController.PlayerState.vSpeed = 0f;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player exit floor collider");
            PlayerController.PlayerState.inAir = true;
        }
    }
}
