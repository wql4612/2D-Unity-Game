using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //常量
    public const float gravity = 15f;
    public const friction = 10f;
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
        public static float hMAxSpeed = 8f;
        public static float hAcceleration = 25f;
        public static float hFriction = 10f;
        public static float vAcceleration = 10f;
        //角色变动属性
        public static bool faceLeft = false;
        public static float hSpeed = 0f;//水平速度，正值表示向左，负值表示向右
        public static float vSpeed = 0f;//垂直速度，正值表示向上，负值表示向下
    }
    // Start is called before the first frame update
    void Start()
    {
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
            if(PlayerState.hSpeed > -PlayerState.hMAxSpeed){
                PlayerState.hSpeed -= PlayerState.hAcceleration * Time.deltaTime;
            }
        }
        else if(Input.GetKey(InputKeys.Right))
        {
            PlayerState.faceLeft = false;
            if(PlayerState.hSpeed < PlayerState.hMAxSpeed){
                PlayerState.hSpeed += PlayerState.hAcceleration * Time.deltaTime;
            }
        }
        //垂直输入处理
        if(Input.GetKeyDown(InputKeys.Jump))
        {
            PlayerState.vSpeed = PlayerState.vAcceleration;
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
        PlayerState.vSpeed -= gravity * Time.deltaTime;
        //摩擦
        if(PlayerState.vSpeed == 0)//角色在地面上时才判定有摩檫力
        {
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
        }

        //角色运动计算
        Vector2 vector2 = transform.position;//获取角色位置
        vector2.x += PlayerState.hSpeed * Time.deltaTime;
        vector2.y += PlayerState.vSpeed * Time.deltaTime;
        //角色位置限制
        if(vector2.x < -10f){
            vector2.x = -10f;
            PlayerState.hSpeed = 0f;
        }
        if(vector2.x > 10f){
            vector2.x = 10f;
            PlayerState.hSpeed = 0f;
        }
        if(vector2.y < -5f){
            vector2.y = -5f;
            PlayerState.vSpeed = 0f;
        }
        if(vector2.y > 5f){
            vector2.y = 5f;
            PlayerState.vSpeed = 0f;
        }
        transform.position = vector2;//设置角色位置
    }
}
