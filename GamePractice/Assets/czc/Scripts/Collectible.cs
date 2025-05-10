using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayerCollected()
    {
        // 触发 Animator 参数
        Debug.Log("Item collected");
        animator.SetTrigger("collect");

        // （可选）如果你要在动画播放完后销毁物体：
        Destroy(gameObject, 1f); // 替换成你的动画长度

        
    }
}