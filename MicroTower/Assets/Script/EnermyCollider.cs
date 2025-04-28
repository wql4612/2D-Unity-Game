using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyCollider : MonoBehaviour
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
        Debug.Log("Enermy Collider Trigger Entered");
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player entered enermy collider");
            PlayerController.PlayerState.inAir = true;
            PlayerController.PlayerState.vSpeed = 0f;
            PlayerController.PlayerState.hSpeed = 0f;
            PlayerController.rb.rotation = 90f;
        }
    }
}
