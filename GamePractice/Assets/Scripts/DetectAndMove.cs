using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetectAndMove : MonoBehaviour
{
    //说明：这个脚本应添加在父对象上，父对象本身的BoxCollider作为Trigger，用于检测是否进入触发范围，然后移动到目标位置。
    //要求该父对象具有至少三个对象：beginPos、endPos、object
    //初始化时，总是将子对象的object放在beginPos的位置，并将目标位置设置为beginPos的位置。
    //若角色触发moveTrigger，则触发移动函数
    [SerializeField] private Transform beginPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private GameObject moveObj;
    [SerializeField] private BoxCollider2D moveTrigger;
    
    [SerializeField] private float speed = 5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        if (!moveTrigger){
            Debug.LogError("MoveTrigger is not assigned!");
        }
        moveObj.transform.position = beginPos.position;
        targetPosition = beginPos.position;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            moveObj.transform.position = Vector3.MoveTowards(
                moveObj.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(moveObj.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                moveTrigger.enabled = true; 
                Debug.Log("Obj reached end position!");
            }
        }
    }
    private void InitializeComponents()
    {
        // 自动获取未手动赋值的组件
        if (!beginPos) beginPos = transform.Find("BeginPos");
        if (!endPos) endPos = transform.Find("EndPos");
        if (!moveTrigger) moveTrigger = GetComponent<BoxCollider2D>();
        if (!moveObj) moveObj = transform.Find("moveObj").gameObject;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && !isMoving)
        {
            StartMovement();
        }
    }

    private void StartMovement()
    {
        // 切换目标位置
        targetPosition =endPos.position;
        isMoving = true;
        moveTrigger.enabled = false; 
        Debug.Log("Wall moving!");
    }
}
