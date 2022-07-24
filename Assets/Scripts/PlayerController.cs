using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace SoulLike
{
    class KeyMapping
    {
        public string keyUp = "w";
        public string keyDown = "s";
        public string keyLeft = "a";
        public string keyRight = "d";
        public string keyJump = "f";
        public string keyShift = "left shift";
        public string keyRoll = "space";
        public string keyAttackL = "mouse 0";
        public string keyAttackR = "mouse 1";
    }
    public class PlayerController : MonoBehaviour
    {
       // public GameObject enemy;
        public Animator anim_Player;
        CharacterController controller;
        AnimatorStateInfo animatorInfo;
        KeyMapping keyMapping = new KeyMapping();
        CharacterStats characterStats;
        Transform groundCheck;
        public LayerMask layerMask;
        public bool inputEnablad = true;
        public bool lockPlanar = false;
        public bool isDead;
        bool IsMoveStop = false;
        bool IsRunStart = false;
        bool runStart=false;
        bool isGround;
        bool run;
        bool rollOver=true;
        bool attackOver = true;
        public bool attackOverB;
        public bool comboPossibleL;
        public bool comboL;
        public bool comboR;
        public bool comboPossibleR;
        
        public int comboStep;

        GameObject rePlay;

        float Dup;
        float Dright;
        float velocityDup;
        float velocityDright;
        float Dmag;
        float gravity = -19.6f;
        float checkRadius = 0.2f;
        float jumpHeight = 2f;

        Vector3 Dvec;
        Vector3 planarVec;
        Vector3 deltaPos;
        Vector3 velocity_g = Vector3.zero;

        public string[] player_AttackL = { "Longs_Attack_p_L", "Longs_Attack_p_R", "Longs_Attack_LD", "Longs_Attack_RD" };
        public string[] player_AttackR = { "Longs_Attack_RD2", "Longs_Attack_Thrust2", "Longs_Attack_D2" };
        private void Awake()
        {
            GameObject player = GameObject.Find("Vampire");
            anim_Player = player.GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            groundCheck = GameObject.Find("IsGround").GetComponent<Transform>();
            characterStats = GetComponent<CharacterStats>();
            rePlay = GameObject.Find("RePlay");
            rePlay.SetActive(false);
        }
        private void Start()
        {
            GameManager.Instance.RigisterPlayer(characterStats);
        }
        private void Update()
        {
            PlayerDeath();
            Gravity();
            AnimatorMoveLock();
            PlayMove();
            PlayMoveStop();
            PlayJump();
            PlayRoll();
            PlayAttack();
        }
        //移动
        private void PlayMove()
        {
            float targetDup = (Input.GetKey(keyMapping.keyUp) ? 1.0f : 0) - (Input.GetKey(keyMapping.keyDown) ? 1.0f : 0);
            float targetDright = (Input.GetKey(keyMapping.keyRight) ? 1.0f : 0) - (Input.GetKey(keyMapping.keyLeft) ? 1.0f : 0);
            if (!inputEnablad)
            {
                targetDup = 0;
                targetDright = 0;
            }
            Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
            Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);
            Vector2 tempDAxis = GameStatic.SquareToCircle(new Vector2(Dup, Dright));
            float Dup2 = tempDAxis.x;
            float Dright2 = tempDAxis.y;
            Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
            Dvec = Dright2 * transform.right + Dup2 * transform.forward;
            if (Dmag > 0.1f)
            {
                anim_Player.transform.forward = Vector3.Slerp(anim_Player.transform.forward, Dvec, 0.1f);
            }
            if (isGround)
            {
                run = Input.GetKey(keyMapping.keyShift);
            }
            anim_Player.SetBool("RunStart", IsRunStart);
            if (run&&runStart==false)
            {
                IsRunStart = true;
                runStart = true;
            }
            if (Input.GetKeyUp(keyMapping.keyShift))
            {
                IsRunStart = false;
                runStart = false;
            }
            float walkSpeed = 2.0f;
            float runSpeed = 4.0f;
            if (!lockPlanar)
            {
                anim_Player.SetFloat("Speed", Dmag * ((run) ? 2.0f : 1.0f), 0.15f, Time.deltaTime);
                planarVec = Dmag * anim_Player.transform.forward * ((run) ? runSpeed : walkSpeed) * Time.deltaTime;
            }
            controller.Move(planarVec+deltaPos);
            deltaPos = Vector3.zero;
        }
        //移动停止
        private void PlayMoveStop()
        {
            anim_Player.SetBool("MoveStop", IsMoveStop);
            animatorInfo = anim_Player.GetCurrentAnimatorStateInfo(0);
            if (animatorInfo.normalizedTime > 1.0f && (animatorInfo.IsName("WalkFwdStop_RU")|| animatorInfo.IsName("SprintStop_RU")|| animatorInfo.IsName("SprintStart")))
            {
                IsMoveStop = false;
                IsRunStart = false;
            }
            if (Input.GetKeyUp(keyMapping.keyUp) || Input.GetKeyUp(keyMapping.keyDown) || Input.GetKeyUp(keyMapping.keyLeft) || Input.GetKeyUp(keyMapping.keyRight))
            {
                if (!GameStatic.CheckState(anim_Player, "WalkFwdStop_RU"))
                {
                    IsMoveStop = true;

                }
                if (!GameStatic.CheckState(anim_Player, "SprintStop_RU") )
                {
                    IsMoveStop = true;

                }
            }
            if (Input.GetKey(keyMapping.keyUp) || Input.GetKey(keyMapping.keyDown) || Input.GetKey(keyMapping.keyLeft) || Input.GetKey(keyMapping.keyRight))
            {
                IsMoveStop = false;
            }
        } 
        //跳跃
        private void PlayJump()
        {
            anim_Player.SetBool("IsGround", isGround);
            if (isGround && Input.GetKeyDown(keyMapping.keyJump)&&rollOver==true&&inputEnablad==true)
            {
                velocity_g.y += Mathf.Sqrt(jumpHeight * (-2) * gravity);
                anim_Player.CrossFade("Jump_Platformer_Start", 0);
            }
        }
        //翻滚
        private void PlayRoll()
        {
            anim_Player.SetBool("RollOver", rollOver); 
            if ((Input.GetKeyDown(keyMapping.keyRoll)&&isGround&&rollOver==true&&inputEnablad==true))
            {
                anim_Player.CrossFade("RollFwd", 0);
                inputEnablad = false;
                rollOver = false;
            }
            if (Input.anyKey)
            {
                //翻滚结束
                if (GameStatic.CheckState(anim_Player, "RollFwd") && inputEnablad == true)
                {
                    rollOver = true;
                }
            }
        }
        private void PlayAttack()
        {
            anim_Player.SetBool("AttackOver", attackOver);
            if (animatorInfo.normalizedTime > 1.0f && (animatorInfo.IsTag("Attack")))
            {
                attackOver = true;
            }
            if (Input.GetKeyDown(keyMapping.keyAttackL)&& rollOver == true&&isGround==true)
            {
                inputEnablad = false;
                attackOver = false;
                attackOverB = false;
                if (comboStep == 0)
                {
                    anim_Player.CrossFade(player_AttackL[0], 0);
                    comboStep = 1;
                }
                if (comboStep != 0)
                {
                    if (comboPossibleL)
                    {
                        comboPossibleL = false;
                        comboL = true;
                        comboStep += 1;
                    }
                    if (comboL == true)
                    {
                        if (comboStep == 2)
                        {
                            anim_Player.CrossFade(player_AttackL[1], 0.1f);
                        }
                        if (comboStep == 3)
                        {
                            anim_Player.CrossFade(player_AttackL[2], 0.1f);
                        }
                        if (comboStep == 4)
                        {
                            anim_Player.CrossFade(player_AttackL[3], 0.1f);
                        }
                        comboL = false;
                    }
                }
            }
            if (Input.GetKeyDown(keyMapping.keyAttackR) && rollOver == true)
            {
                inputEnablad = false;
                attackOver = false;
                attackOverB = false;
                if (comboStep == 0)
                {
                    anim_Player.CrossFade(player_AttackR[0], 0);
                    comboStep = 1;
                }
                if (comboStep != 0)
                {
                    if (comboPossibleR)
                    {
                        comboPossibleR = false;
                        comboR = true;
                        comboStep += 1;
                    }
                    if (comboR == true)
                    {
                        if (comboStep == 2)
                        {
                            anim_Player.CrossFade(player_AttackR[1], 0.1f);
                        }
                        if (comboStep == 3)
                        {
                            anim_Player.CrossFade(player_AttackR[2], 0.1f);
                        }
                        comboR = false;
                    }
                }
            }
            if (Input.anyKey)
            {
                //攻击结束
                if (GameStatic.CheckStateTag(anim_Player, "Attack") && attackOverB==true)
                {
                    attackOver = true;
                    comboPossibleL = false;
                    comboPossibleR = false;
                    comboStep = 0;
                }
            }
        }
        void PlayerDeath()
        {
            isDead = characterStats.CurrentHealth == 0;
            anim_Player.SetBool("Death", isDead);
            if (isDead)
            {
                controller.enabled = false;
                GameManager.Instance.NotifyObserVers();
                Cursor.visible = true;//隐藏指针
                Cursor.lockState = CursorLockMode.None;
                rePlay.SetActive(true);
                rePlay.GetComponent<Button>().onClick.AddListener(RePlayGame);
            }
        }

        void RePlayGame()
        {
            SceneManager.LoadScene(0);
        }
        //使用根动画
        public void AnimatorRootMotion(object _deltaPos)
        {
            if (GameStatic.CheckStateTag(anim_Player, "Root")|| GameStatic.CheckStateTag(anim_Player, "Attack"))
            {
                deltaPos += (Vector3)_deltaPos;
            }
        }
        //状态锁定
        private void AnimatorMoveLock()
        {
            if (!isGround)
            {
                IsMoveStop = false;
                IsRunStart = false;
            }
        }
        //重力
        private void Gravity()
        {
            velocity_g.y += gravity * Time.deltaTime;
            controller.Move(velocity_g * Time.deltaTime);
            isGround = Physics.CheckSphere(groundCheck.position, checkRadius, layerMask);
            if (isGround&&velocity_g.y<0)
            {
                velocity_g.y = 0;
            }
        }
    }
}

