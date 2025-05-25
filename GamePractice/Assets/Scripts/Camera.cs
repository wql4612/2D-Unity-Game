using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 fixedPosition;
    [SerializeField] private bool followPlayer_x = true;
    [SerializeField] private bool followPlayer_y = true;
    [SerializeField] private float smoothTime = 0.2f; 

    private Vector3 currentVelocity; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = transform.position;

        if (player != null)
        {
            // 分别判断x/y轴是否跟随
            float targetX = followPlayer_x ? player.position.x : fixedPosition.x;
            float targetY = followPlayer_y ? player.position.y : fixedPosition.y;
            float targetZ = transform.position.z;

            targetPosition = new Vector3(targetX, targetY, targetZ);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
    }

    public void ToggleFollowPlayerX(bool follow)
    {
        followPlayer_x = follow;
    }
    public void ToggleFollowPlayerY(bool follow)
    {
        followPlayer_y = follow;
    }

    public void SetFixedPosition(Vector3 position)
    {
        Debug.Log("Setting fixed position to " + position);
        fixedPosition = new Vector3(position.x, position.y, transform.position.z);
        Debug.Log("Fixed position set to " + fixedPosition);
    }
}