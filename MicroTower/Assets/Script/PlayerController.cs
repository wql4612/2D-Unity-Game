using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //常量
    public const float gravity = 10f;
    public const float friction = 10f;
    //系统属性 
    public static Rigidbody2D rb;
    public static CapsuleCollider2D cc;
    public class InputKeys
    {
    //定义输入的按键
        public static KeyCode Left = KeyCode.A;
        public static KeyCode Right = KeyCode.D;
        public static KeyCode Jump = KeyCode.W;
        public static KeyCode Squat = KeyCode.S;
    }
    public class PlayerState
    {
        //角色自身属性
        public static float hMaxSpeed = 8f;
        public static float hAcceleration = 25f;
        public static float hFriction = 10f;
        public static float vAcceleration = 5f;
        //角色变动属性
        public static bool faceLeft = false;//角色朝向，true表示向左，false表示向右
        public static bool inAir= true;//角色是否在空中
        public static float hSpeed = 0f;//水平速度，正值表示向左，负值表示向右
        public static float vSpeed = 0f;//垂直速度，正值表示向上，负值表示向下
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        Application.targetFrameRate = 60;//设置帧率
    }

    // Update is called once per frame
    void Update()
    {
        //水平输入处理
        if(Input.GetKey(InputKeys.Left) && Input.GetKey(InputKeys.Right)){}
        else if(Input.GetKey(InputKeys.Left))
        {
            PlayerState.faceLeft = true;
            if(PlayerState.hSpeed > -PlayerState.hMaxSpeed){
                PlayerState.hSpeed -= PlayerState.hAcceleration * Time.deltaTime;
            }
        }
        else if(Input.GetKey(InputKeys.Right))
        {
            PlayerState.faceLeft = false;
            if(PlayerState.hSpeed < PlayerState.hMaxSpeed){
                PlayerState.hSpeed += PlayerState.hAcceleration * Time.deltaTime;
            }
        }
        //垂直输入处理
        if(Input.GetKeyDown(InputKeys.Jump))
        {
            if(!PlayerState.inAir)
            {
                PlayerState.vSpeed = PlayerState.vAcceleration;
                PlayerState.inAir = true;
            }
        }
        if(Input.GetKey(InputKeys.Squat))
        {
            if(PlayerState.hSpeed != 0 && PlayerState.vSpeed == 0)//角色处于地面上时，若蹲下，则水平速度急剧减小
            {
                PlayerState.hSpeed = PlayerState.hSpeed * 0.5f;
            }
        }
        //环境影响处理
        //重力
        if(PlayerState.inAir)
        {
            PlayerState.vSpeed -= gravity * Time.deltaTime;
        }
        //摩擦
            if(PlayerState.hSpeed > 0)
            {
                if(PlayerState.hSpeed > friction * Time.deltaTime)
                {
                    PlayerState.hSpeed -= friction * Time.deltaTime;
                }
                else
                {
                    PlayerState.hSpeed = 0f;
                }
            }
            else if(PlayerState.hSpeed < 0)
            {
                if(PlayerState.hSpeed < -friction * Time.deltaTime)
                {
                    PlayerState.hSpeed += friction * Time.deltaTime;
                }
                else
                {
                    PlayerState.hSpeed = 0f;
                }
            }

        //角色运动计算
        Vector2 move = new Vector2(PlayerState.hSpeed, PlayerState.vSpeed);
        rb.velocity = move;
        //角色朝向设置
        if(PlayerState.faceLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

}
