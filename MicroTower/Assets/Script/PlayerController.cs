using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PlayerController : MonoBehaviour
{
    //常量
    public const float gravity = 10f;
    public const float friction = 10f;
    public static Rigidbody2D rb;
    public static CapsuleCollider2D cc;
    public static SpriteRenderer sr;
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
        //角色基础属性
        public static float hMaxSpeed = 8f;
        public static float hAcceleration = 25f;
        public static float hFriction = 10f;
        public static float vAcceleration = 5f;
        public static int maxHealth = 100;
        //角色变动属性
        public static float hurtInterval = 0f;//受伤间隔时间
        public static float blinkTime = 0f;//闪烁时间
        public static bool isAlive = true;//角色是否存活
        public static bool faceLeft = false;//角色朝向，true表示向左，false表示向右
        public static bool inAir= true;//角色是否在空中
        public static bool onLeftWall = false;//角色是否在墙壁上
        public static bool onRightWall = false;//角色是否在墙壁上
        public static float hSpeed = 0f;//水平速度，负值表示向左，正值表示向右
        public static float vSpeed = 0f;//垂直速度，正值表示向上，负值表示向下
        public static int health = maxHealth;//角色生命值
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 60;//设置帧率
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerState.isAlive){
            //不接收输入，或者转入其他输入点
        }else{
            //水平输入处理
            if(Input.GetKey(InputKeys.Left) && Input.GetKey(InputKeys.Right)){}
            else if(Input.GetKey(InputKeys.Left) && PlayerState.onLeftWall == false)
            {
                PlayerState.faceLeft = true;
                if(PlayerState.hSpeed > -PlayerState.hMaxSpeed){
                    PlayerState.hSpeed -= PlayerState.hAcceleration * Time.deltaTime;
                }else
                {
                    PlayerState.hSpeed = -PlayerState.hMaxSpeed;
                }
            }
            else if(Input.GetKey(InputKeys.Right) && PlayerState.onRightWall == false)
            {
                PlayerState.faceLeft = false;
                if(PlayerState.hSpeed < PlayerState.hMaxSpeed){
                    PlayerState.hSpeed += PlayerState.hAcceleration * Time.deltaTime;
                }else
                {
                    PlayerState.hSpeed = PlayerState.hMaxSpeed;
                }
            }
            //垂直输入处理
            if(Input.GetKeyDown(InputKeys.Jump))
            {
                if(!PlayerState.inAir)
                {
                    PlayerState.vSpeed = PlayerState.vAcceleration;
                }
                else if(PlayerState.onLeftWall)
                {
                    PlayerState.vSpeed = PlayerState.vAcceleration;
                    PlayerState.hSpeed = PlayerState.hMaxSpeed;
                    PlayerState.faceLeft = false;
                }
                else if(PlayerState.onRightWall)
                {
                    PlayerState.vSpeed = PlayerState.vAcceleration;
                    PlayerState.hSpeed = -PlayerState.hMaxSpeed;
                    PlayerState.faceLeft = true;
                }
            }
            if(Input.GetKey(InputKeys.Squat))
            {
                if(PlayerState.hSpeed != 0 && PlayerState.vSpeed == 0)//角色处于地面上时，若蹲下，则水平速度急剧减小
                {
                    PlayerState.hSpeed = PlayerState.hSpeed * 0.5f;
                }
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
        //若角色有受伤，则计算受伤间隔
        if (PlayerState.hurtInterval > 0)
        {
            if(PlayerState.isAlive)
            {
                PlayerState.blinkTime += Time.deltaTime;
                if(PlayerState.blinkTime >= 0f)
                {
                    sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,0.3f);
                }else{
                    sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,1f);
                }
                if(PlayerState.blinkTime >= 0.2f)
                {
                    PlayerState.blinkTime = -0.2f;
                }
                PlayerState.hurtInterval -= Time.deltaTime;
            }
            if (PlayerState.hurtInterval <= 0)
            {
                Debug.Log("Hurt interval end");
                sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,1f);
                PlayerState.blinkTime = 0f;
            }
        }
    }
    public static void Hurt()//角色受伤处理
    {
        //反转运动方向
        PlayerState.hSpeed = -PlayerState.hSpeed*1.1f;
        PlayerState.vSpeed = -PlayerState.vSpeed*1.1f;
        if(PlayerState.hurtInterval <= 0)
        {
            Debug.Log("Get hurt");
            PlayerState.hurtInterval = 3f;
            if(PlayerState.isAlive)
            {
                PlayerState.health -= 20;
                if(PlayerState.health <= 0)
                {
                    Kill();
                }
                
            }
        }
    }
    public static void Kill()//角色死亡处理
    {
        Debug.Log("Player dead");
        PlayerState.isAlive = false;
        rb.rotation = 90;
        sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,0.3f);
    }

}
