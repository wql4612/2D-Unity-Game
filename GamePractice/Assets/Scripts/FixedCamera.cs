using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    [SerializeField] private float target_x;
    [SerializeField] private float target_y;
    [SerializeField] private float scale_change_to = 5;
    [SerializeField] private bool fixX = true;
    [SerializeField] private bool fixY = true;
    private float old_scale;
    private CameraFollow camera_follow;
    
    // Start is called before the first frame update
    void Start()
    {
        camera_follow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
        old_scale = camera_follow.GetComponents<Camera>()[0].orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;
        camera_follow.ToggleFollowPlayerX(!fixX); // true=跟随，false=锁定
        camera_follow.ToggleFollowPlayerY(!fixY);

        Vector3 camPos = camera_follow.transform.position;
        float x = fixX ? target_x : camPos.x;
        float y = fixY ? target_y : camPos.y;
        camera_follow.SetFixedPosition(new Vector3(x, y, camPos.z));
        camera_follow.GetComponent<Camera>().orthographicSize = scale_change_to;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;
        camera_follow.ToggleFollowPlayerX(true);
        camera_follow.ToggleFollowPlayerY(true);
        camera_follow.GetComponent<Camera>().orthographicSize = old_scale;
    }
}
