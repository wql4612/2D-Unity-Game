using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffFollower : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {// Follow the player's x-position 
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
    }
}
