using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    private bool isActive = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            isActive = true;
            SaveCheckPointData();
            TriggerActivationAnimation();
        }
    }

    private void SaveCheckPointData()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        PlayerPrefs.SetFloat("CheckpointX", playerPos.x);
        PlayerPrefs.SetFloat("CheckpointY", playerPos.y);
        PlayerPrefs.SetString("CheckpointScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    private void TriggerActivationAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("FlagTrans"); // 播放一次切换动画
        }
    }

    // 可在动画中设置事件回调（推荐）调用这个
    public void OnFlagTransFinished()
    {
        if (animator != null)
        {
            animator.Play("FlagIdle"); // 切换到Idle状态，持续播放
        }
    }
}
