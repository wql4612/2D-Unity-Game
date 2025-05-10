using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 fixedPosition;
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private float smoothTime = 0.2f; 

    private Vector3 currentVelocity; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition;

        if (followPlayer)
        {

            targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
        else
        {

            targetPosition = fixedPosition;
        }


        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
    }

    public void ToggleFollowPlayer(bool follow)
    {
        followPlayer = follow;
    }

    public void SetFixedPosition(Vector3 position)
    {
        Debug.Log("Setting fixed position to " + position);
        fixedPosition= new Vector3(position.x, position.y, transform.position.z);
        Debug.Log("Fixed position set to " + fixedPosition);
    }
}