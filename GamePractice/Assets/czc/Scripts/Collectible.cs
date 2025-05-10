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
        // ���� Animator ����
        Debug.Log("Item collected");
        animator.SetTrigger("collect");

        // ����ѡ�������Ҫ�ڶ�����������������壺
        Destroy(gameObject, 1f); // �滻����Ķ�������

        
    }
}