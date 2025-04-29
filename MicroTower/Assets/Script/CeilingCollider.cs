using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //注意：天花板仅将角色的垂直速度降低为0，且不可加入Stay判定，否则会导致悬空
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
            PlayerController.PlayerState.vSpeed = 0f;
        }
    }
}
