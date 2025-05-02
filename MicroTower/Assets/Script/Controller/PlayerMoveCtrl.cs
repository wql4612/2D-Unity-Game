using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;

namespace MicroTower
{
    public class PlayerMoveCtrl : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float wallSlideSpeed = 2f; // 新增：墙壁滑落速度
        
        [Header("Gravity Settings")]
        [SerializeField] private float gravityScale = 1f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        
        [Header("Collision Checks")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform leftCheck;
        [SerializeField] private Transform rightCheck;
        // [SerializeField] private Transform ceilingCheck;
        [SerializeField] private float checkRadius = 0.15f;
        [SerializeField] private float wallCheckDistance = 0.1f; // 新增：墙壁检测距离
        
        [Header("Layer Masks")]
        [SerializeField] private LayerMask blockLayer; // 更名：Ground -> Block
        [SerializeField] private LayerMask wallLayer; // 新增：单独墙壁层
        // [SerializeField] private LayerMask ceilingLayer; // 新增：天花板层
        [Header("Character States")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool isTouchingLeftWall;
        [SerializeField] private bool isClimbingLeftWall;
        [SerializeField] private bool isTouchingRightWall;
        [SerializeField] private bool isClimbingRightWall;
        [SerializeField] private bool isTouchingCeiling;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float horizontalInputDisableTimer = 0f; // 新增：水平输入禁用计时器
        [SerializeField] private float moveInput; // 新增：水平方向输入
        private Rigidbody2D rb;
        

        
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            
            // 初始化所有检测点
            InitializeCheckPoint(ref groundCheck, "GroundCheck", Vector3.down * 0.5f);
            InitializeCheckPoint(ref leftCheck, "LeftCheck", Vector3.left * 0.3f);
            InitializeCheckPoint(ref rightCheck, "RightCheck", Vector3.right * 0.3f);
            // InitializeCheckPoint(ref ceilingCheck, "CeilingCheck", Vector3.up * 0.5f);
            
            // 检查层级设置
            if(blockLayer.value == 0)
            {
                blockLayer = LayerMask.GetMask("Block");
                if(blockLayer.value == 0) Debug.LogError("Block layer not found!");
            }
            if(wallLayer.value == 0) wallLayer = blockLayer; // 默认使用Block层
            // if(ceilingLayer.value == 0) ceilingLayer = blockLayer;
        }

        // 初始化检测点辅助方法
        void InitializeCheckPoint(ref Transform checkPoint, string name, Vector3 localPos)
        {
            if (checkPoint == null)
            {
                checkPoint = transform.Find(name);
                if (checkPoint == null)
                {
                    GameObject checkObj = new GameObject(name);
                    checkObj.transform.SetParent(transform);
                    checkObj.transform.localPosition = localPos;
                    checkPoint = checkObj.transform;
                }
            }
        }

        void UpdateCollisionChecks()
        {
            // 地面检测（圆形）
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, blockLayer);
            
            // 墙壁检测（射线）
            isTouchingLeftWall = Physics2D.Raycast(
                leftCheck.position, Vector2.left, wallCheckDistance, wallLayer);
            
            isTouchingRightWall = Physics2D.Raycast(
                rightCheck.position, Vector2.right, wallCheckDistance, wallLayer);
            
            // // 天花板检测（圆形）
            // isTouchingCeiling = Physics2D.OverlapCircle(
            //     ceilingCheck.position, checkRadius, ceilingLayer);
        }

        void Update()
        {
            UpdateCollisionChecks();
            
            // 跳跃输入（检测地面或墙壁）
            if (Input.GetKey(KeyCode.W))
            {
                if(isGrounded)
                {
                    velocity.y = jumpForce;
                }
                rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            }
            if(Input.GetKeyDown(KeyCode.W))
            {
                if(isClimbingLeftWall || isClimbingRightWall)
                {
                    // 墙跳：竖直方向速度 + 水平方向速度
                    velocity.y = jumpForce;
                    // 左侧墙向右跳，右侧墙向左跳
                    velocity.x = (isClimbingLeftWall ? 1 : -1) * jumpForce;
                    
                    // 应用移动
                    rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
                    Debug.Log(velocity);
                    // 启动水平输入禁用计时器
                    horizontalInputDisableTimer = 0.2f;
                }
            }
        }
        
        void FixedUpdate()
        {
            
            moveInput = Input.GetAxis("Horizontal");

            // 如果触碰左墙，则忽略向左的输入
            if (moveInput < 0 && isTouchingLeftWall)
            {
                moveInput = 0;
            }
            // 如果触碰右墙，则忽略向右的输入
            else if (moveInput > 0 && isTouchingRightWall)
            {
                moveInput = 0;
            }
            if (horizontalInputDisableTimer > 0f)
            {    
                horizontalInputDisableTimer -= Time.fixedDeltaTime;
            }
            else{
                velocity.x = moveInput * moveSpeed;
            }
            //重置Climbing状态
            isClimbingLeftWall = false;
            isClimbingRightWall = false;
            if(Input.GetKey(KeyCode.K))
            {
                if(isTouchingLeftWall)
                {
                    isClimbingLeftWall = true;
                }
                else if(isTouchingRightWall)
                {
                    isClimbingRightWall = true;
                }
            }
            // 墙壁滑动效果
            if((isTouchingLeftWall|| isTouchingRightWall)  && !isGrounded && velocity.y < 0)
            {
                if(isClimbingLeftWall || isClimbingRightWall)
                    velocity.y = 0;
                else
                    velocity.y = -wallSlideSpeed; // 控制墙壁下滑速度
            }
            
            // 自定义重力系统
            if (!isGrounded)
            {
                if (velocity.y < 0) // 下落加速
                {
                    velocity += Physics2D.gravity * (fallMultiplier * gravityScale) * Time.fixedDeltaTime;
                }
                else if (velocity.y > 0 && !Input.GetKey(KeyCode.W)) // 小跳加速下落
                {
                    velocity += Physics2D.gravity * (lowJumpMultiplier * gravityScale) * Time.fixedDeltaTime;
                }
                else // 普通重力
                {
                    velocity += Physics2D.gravity * gravityScale * Time.fixedDeltaTime;
                }
                
                // // 天花板碰撞检测
                // if(isTouchingCeiling && velocity.y > 0)
                // {
                //     velocity.y = -velocity.y;
                // }
            }
            else if (velocity.y < 0) // 地面接触
            {
                velocity.y = 0;
            }
            
            // 应用移动
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        
        private void OnDrawGizmosSelected()
        {
            // 地面检测
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
            }
            
            // 墙壁检测
            if(leftCheck != null)
            {
                Gizmos.color = isTouchingLeftWall ? Color.blue : Color.white;
                Gizmos.DrawLine(leftCheck.position, leftCheck.position + Vector3.left * wallCheckDistance);
            }
            
            if(rightCheck != null)
            {
                Gizmos.color = isTouchingRightWall ? Color.blue : Color.white;
                Gizmos.DrawLine(rightCheck.position, rightCheck.position + Vector3.right * wallCheckDistance);
            }
            
            // 天花板检测
            // if(ceilingCheck != null)
            // {
            //     Gizmos.color = isTouchingCeiling ? Color.yellow : Color.white;
            //     Gizmos.DrawWireSphere(ceilingCheck.position, checkRadius);
            // }
        }
    }
}
