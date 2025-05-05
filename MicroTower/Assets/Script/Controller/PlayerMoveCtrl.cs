using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;

namespace MicroTower
{
    public class PlayerMoveCtrl : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Transform characterSkin;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float wallSlideSpeed = 3f; // 新增：墙壁滑落速度
        [SerializeField] private float climbSpeed = 3f; // 爬墙速度
        
        [Header("Gravity Settings")]
        [SerializeField] private float gravityScale = 1f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        
        [Header("Collision Checks")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform leftCheck;
        [SerializeField] private Transform rightCheck;
        [SerializeField] private Transform ceilingCheck;
        [SerializeField] private float checkRadius = 0.2f;
        [SerializeField] private float wallCheckDistance = 0.1f; // 新增：墙壁检测距离
        
        [Header("Layer Masks")]
        [SerializeField] private LayerMask blockLayer; // 更名：Ground -> Block
        [SerializeField] private LayerMask wallLayer; // 新增：单独墙壁层
        [Header("Character States")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool isTouchingLeftWall;
        [SerializeField] private bool isClimbingLeftWall;
        [SerializeField] private bool isTouchingRightWall;
        [SerializeField] private bool isClimbingRightWall;
        [SerializeField] private bool isTouchingCeiling;
        [SerializeField] private bool isWalking;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float horizontalInputDisableTimer = 0f; // 新增：水平输入禁用计时器
        [SerializeField] private float moveInput; // 新增：水平方向输入

        [Header("Animation Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private string walkAnimParam = "IsWalking";
        [SerializeField] private string climbAnimParam = "IsClimbing";
        [SerializeField] private string climbSpeedParam = "ClimbSpeed";
        private Rigidbody2D rb;
        

        
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            GameObject skin = transform.Find("Skin").gameObject;
            if (skin != null)
            {
                characterSkin = skin.transform;
                if (characterSkin == null)
                {
                    Debug.LogError("Character skin not found!");
                }
                animator = skin.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError("Animator not found!");
                }
            }
            
            // 初始化所有检测点
            InitializeCheckPoint(ref groundCheck, "GroundCheck", Vector3.down * 1);
            InitializeCheckPoint(ref leftCheck, "LeftCheck", Vector3.left * 0.5f);
            InitializeCheckPoint(ref rightCheck, "RightCheck", Vector3.right * 0.5f);
            InitializeCheckPoint(ref ceilingCheck, "CeilingCheck", Vector3.up * 0.5f);
            
            // 检查层级设置
            if(blockLayer.value == 0)
            {
                blockLayer = LayerMask.GetMask("Block");
                if(blockLayer.value == 0) Debug.LogError("Block layer not found!");
            }
            if(wallLayer.value == 0) wallLayer = blockLayer; // 默认使用Block层

            animator = characterSkin.GetComponent<Animator>();
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
            // 天花板检测
            isTouchingCeiling = Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, blockLayer);
        }

        void Update()
        {
            UpdateCollisionChecks();
            
            // 跳跃输入（检测地面或墙壁）
            if (Input.GetKey(KeyCode.J))
            {
                if(isGrounded)
                {
                    velocity.y = jumpForce;
                }
                rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            }
            if(Input.GetKeyDown(KeyCode.J))
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
            if (moveInput > 0 && isTouchingRightWall)
            {
                moveInput = 0;
            }
            if(isClimbingLeftWall || isClimbingRightWall)
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
            
            // 爬墙移动逻辑
            if (isClimbingLeftWall || isClimbingRightWall)
            {
                float verticalInput = Input.GetAxis("Vertical");
                
                // 向上爬
                if (verticalInput > 0.1f) // 按住W
                {
                    velocity.y = climbSpeed;
                }
                // 向下滑
                else if (verticalInput < -0.1f) // 按住S
                {
                    velocity.y = -climbSpeed;
                }
                // 不按任何键时停在墙上
                else
                {
                    velocity.y = 0;
                }
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
            // // 墙壁滑动效果(只在非攀爬状态下生效)
            // if((isTouchingLeftWall || isTouchingRightWall) && !isGrounded && velocity.y < 0 && !(isClimbingLeftWall || isClimbingRightWall))
            // {
            //     velocity.y = -wallSlideSpeed;
            // }
            
            // 自定义重力系统(只在非攀爬状态下生效)
            if (!isGrounded && !(isClimbingLeftWall || isClimbingRightWall))
            {
                if (velocity.y < 0) // 下落加速
                {
                    velocity += Physics2D.gravity * (fallMultiplier * gravityScale) * Time.fixedDeltaTime;
                }
                else if (velocity.y > 0 && !Input.GetKey(KeyCode.J)) // 小跳加速下落
                {
                    velocity += Physics2D.gravity * (lowJumpMultiplier * gravityScale) * Time.fixedDeltaTime;
                }
                else // 普通重力
                {
                    velocity += Physics2D.gravity * gravityScale * Time.fixedDeltaTime;
                }
            }
            else if (velocity.y < 0 && isGrounded) // 地面接触
            {
                velocity.y = 0;
            }
            
            if(isTouchingCeiling && velocity.y > 0)
            {
                velocity.y = 0;
            }
            
            // 应用移动
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            // 更新角色朝向
            UpdateCharacterFacing();
            // 更新动画状态
            UpdateAnimationState();
        }
        
        // 更新角色朝向方法
        private void UpdateCharacterFacing()
        {
            if (velocity.x > 0.1f || isClimbingRightWall) // 向右移动
            {
                characterSkin.localScale = new Vector3(1.5f,1.5f, 1.5f); // 默认朝向
            }
            else if (velocity.x < -0.1f || isClimbingLeftWall) // 向左移动
            {
                characterSkin.localScale = new Vector3(-1.5f, 1.5f, 1.5f); // 翻转X轴
            }
            // 如果速度接近0则保持当前朝向
        }
        private void UpdateAnimationState()
        {
            // 设置行走动画参数
            isWalking = isGrounded && Mathf.Abs(velocity.x) > 0.01f;
            animator.SetBool(walkAnimParam, isWalking);
            // 设置爬墙动画参数
            bool isClimbing = isClimbingLeftWall || isClimbingRightWall;
            animator.SetBool(climbAnimParam, isClimbing);
            
            if(isClimbing)
            {
                // 向上爬 - 正向播放动画
                if (Input.GetKey(KeyCode.W))
                {
                    animator.SetInteger(climbSpeedParam, 1); // 使用 SetInteger 方法
                }
                // 向下爬 - 反向播放动画
                else if (Input.GetKey(KeyCode.S))
                {
                    animator.SetInteger(climbSpeedParam, -1); // 使用 SetInteger 方法
                }
                // 静止 - 播放Idle动画
                else
                {
                    animator.SetInteger(climbSpeedParam, 0); // 使用 SetInteger 方法
                }
            }
            else
            {
                animator.SetInteger(climbSpeedParam, 0); // 非爬墙状态重置
            }
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
            if(ceilingCheck != null)
            {
                Gizmos.color = isTouchingCeiling ? Color.red : Color.white;
                Gizmos.DrawWireSphere(ceilingCheck.position, checkRadius);
            }
            
        }
    }
}
