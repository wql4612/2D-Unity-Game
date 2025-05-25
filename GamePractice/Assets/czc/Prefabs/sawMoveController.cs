using UnityEngine;
using System.Collections;

public class SawTrapController : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;

    private Vector3 worldPointA;
    private Vector3 worldPointB;
    private Vector3 targetPosition;

    private void Start()
    {
        // 记录初始世界坐标
        worldPointA = pointA.position;
        worldPointB = pointB.position;
        targetPosition = worldPointB;

        StartCoroutine(MoveBetweenPoints());
    }

    private IEnumerator MoveBetweenPoints()
    {
        while (true)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
            yield return new WaitForSeconds(waitTime);

            // 切换目标点
            targetPosition = (targetPosition == worldPointA) ? worldPointB : worldPointA;
        }
    }
}
