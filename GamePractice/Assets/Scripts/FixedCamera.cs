using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    [SerializeField] private float target_x;
    [SerializeField] private float target_y;
    private CameraFollow camera_follow;
    
    // Start is called before the first frame update
    void Start()
    {
        camera_follow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        camera_follow.ToggleFollowPlayer(false);
        camera_follow.SetFixedPosition(new Vector3(target_x, target_y, 0));
    }
}
