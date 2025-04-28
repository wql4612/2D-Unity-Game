using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.3f;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        if(target == null)
        {
            Debug.Log("No target assigned");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            smoothedPosition.z = transform.position.z;
            transform.position = smoothedPosition;
        }
    }
}
