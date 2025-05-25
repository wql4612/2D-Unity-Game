using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneFollowAnother : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform target;
    private Vector3 oldPosition; // 修改为 Vector3 类型
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned to OneFollowAnother script");
            return;
        }
        oldPosition = target.position; // 初始化为 target 的初始位置
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += target.position - oldPosition; // 计算目标对象的移动向量并作用到本体上
        oldPosition = target.position; // 更新 oldPosition 为当前 target 的位置
    }
}
