using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineTrigger : MonoBehaviour
{
    [SerializeField] private float TrampolineForce = 15f;
    [SerializeField] private  bool isTriggered = false;

    [Header("Animation Settings")]
    [SerializeField] private Animator trampolineAnimator;
    [SerializeField] private string activeAnimParam = "IsActive";
    // Start is called before the first frame update
    void Start()
    {
        trampolineAnimator = GetComponent<Animator>();
        if (trampolineAnimator == null)
        {
            Debug.LogError("Animator not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 更新动画状态
        UpdateAnimationState();
    }
    private void UpdateAnimationState()
    {
        if(isTriggered)
        {
            trampolineAnimator.SetBool(activeAnimParam, true);
        }
        else
        {
            trampolineAnimator.SetBool(activeAnimParam, false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trampoline Triggered");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Get");
            if (collision.gameObject.GetComponent<PlayerMovement>().rb.velocity.y < 0)
            {
                collision.gameObject.GetComponent<PlayerMovement>().rb.velocity = new Vector2(collision.gameObject.GetComponent<PlayerMovement>().rb.velocity.x, TrampolineForce);
                isTriggered = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trampoline Exit");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Exit");
            isTriggered = false;
        }
    }
    // 新增方法，用于响应动画事件
    public void ResetTrigger()
    {
        Debug.Log("Reset Trigger");
        isTriggered = false;
    }
}
