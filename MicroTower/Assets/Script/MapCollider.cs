using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCollisper : MonoBehaviour
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
        Debug.Log("Map Collider Trigger Entered");
        PlayerController player = other.GetComponent<PlayerController>();
        if (player!= null)
        {
            Debug.Log("Player entered map collider");
            PlayerController.PlayerState.inAir = false;
            PlayerController.PlayerState.vSpeed = 0f;
        }
    }
}
