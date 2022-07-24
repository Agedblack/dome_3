using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoulLike
{
    public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour,IEndGameObserver
    {
        public GameObject player;
        private EnemyStates enemyStates;
        private CharacterStats characterStats;
        private NavMeshAgent agent;
        private Animator anim;
        private Collider coll_EnemyA;

        [Header("追击相关")]
        public float sightRadius;
        private GameObject attackTarget;
        public bool isGuard;
        private float speed_EnemyA;
        private float lastAttackTime;//攻击间隔

        bool isWalk;
        bool isChase;
        bool isDead;
        public bool playerIsDead;
        [Header("巡逻范围")]
        public float patrolRange;
        public float lookAtTime;//巡逻观察时间
        private float remainLookAtTime;//还需观察的时间
        private Vector3 wayPoint;//随机点
        private Vector3 guardPos;//初始点

        private Quaternion guardRotation;//记录旋转角度
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            characterStats = GetComponent<CharacterStats>();
            anim = GetComponent<Animator>();
            coll_EnemyA = GetComponent<Collider>();
            speed_EnemyA = agent.speed;
            guardPos = transform.position;
            guardRotation = transform.rotation;
            remainLookAtTime = lookAtTime;
        }
        private void Start()
        {
            if (isGuard)
            {
                enemyStates = EnemyStates.GUARD;
            }
            else
            {
                enemyStates = EnemyStates.PATROL;
                GetNewWayPoint();
                GameManager.Instance.AddObserver(this);
            }
        }
        //void OnEnable()
        //{
        //    //启用
        //    GameManager.Instance.AddObserver(this);
        //}
        void OnDisable()
        {
            //禁用
            if (!GameManager.IsInitialized) return;
            GameManager.Instance.RemoveObserver(this);
        }
        private void Update()
        {
            if (characterStats.CurrentHealth == 0)
                isDead = true;
            if (!playerIsDead)
            {
                SwitchStates();
                SwitchAnimation();
                Attack();
                lastAttackTime -= Time.deltaTime;
            }
        }
        void SwitchAnimation()
        {
            anim.SetBool("EnemyA_Walk", isWalk);
            anim.SetBool("EnemyA_Run", isChase);
            anim.SetBool("EnemyA_Critical", characterStats.isCritical);
            anim.SetBool("EnemyA_Death", isDead);
        }
        void SwitchStates()
        {
            if (isDead)
            {
                enemyStates = EnemyStates.DEAD;
            }
            //如果发现player 切换到追击
            else if (FoundPlayer())
            {
                enemyStates = EnemyStates.CHASE;
            }
            switch (enemyStates)
            {
                case EnemyStates.GUARD:
                    isChase = false;
                    if (transform.position != guardPos)
                    {
                        isWalk = true;
                        agent.isStopped = false;
                        agent.destination = guardPos;

                        if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                        {
                            isWalk = false;
                            transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                        }
                    }
                    break;
                case EnemyStates.PATROL:
                    isChase = false;
                    agent.speed = speed_EnemyA * 0.5f;
                    //判断是否到达随机巡逻点
                    if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        if (remainLookAtTime > 0)
                        {
                            remainLookAtTime -= Time.deltaTime;
                        }
                        else
                        {
                            GetNewWayPoint();
                        }
                    }
                    else
                    {
                        isWalk = true;
                        agent.destination = wayPoint;
                    }
                    break;
                case EnemyStates.CHASE:
                    //TODO:追Player
                    //TODO：在攻击范围内则攻击
                    //TODO：配合动画
                    isWalk = false;
                    isChase = true;
                    agent.speed = speed_EnemyA;
                    if (!FoundPlayer())
                    {
                        //TODO：拉脱回到上一个状态
                        isChase = false;
                        agent.isStopped = false;
                        if (remainLookAtTime > 0)
                        {
                            agent.destination = transform.position;
                            remainLookAtTime -= Time.deltaTime;
                        } 
                        else if (isGuard)
                        {
                            enemyStates = EnemyStates.GUARD;
                        }
                        else
                        {
                            enemyStates = EnemyStates.PATROL;
                        }
                    }
                    else
                    {
                        if (GameStatic.CheckStateTag(anim, "Stop"))
                        {
                            agent.isStopped = true;
                        }
                        else
                        {
                            isChase = true;
                            agent.isStopped = false;
                            agent.destination = attackTarget.transform.position;
                        }
                    }
                    //TODO:在攻击范围内则攻击
                    if (TargetInAttackRange() || TargetInSkillRange())
                    {
                        isChase = false;
                        agent.isStopped = true;
                        if (lastAttackTime < 0)
                        {
                            //攻击间隔
                            lastAttackTime = characterStats.attackData.coolDown;
                            //暴击
                            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                            //执行攻击
                        }
                    }
                    break;
                case EnemyStates.DEAD:
                    coll_EnemyA.enabled = false;
                    agent.enabled = false;
                    Destroy(gameObject, 2f);
                    break;
            }
        }
        void Attack()
        {
            if (FoundPlayer())
            {
                transform.LookAt(attackTarget.transform);
            }
            if (TargetInAttackRange())
            {
                //普通攻击
                anim.SetTrigger("EnemyA_Attack");
            }
            if (TargetInSkillRange())
            {
                //特殊攻击
                anim.SetTrigger("EnemyA_Skill");
            }
        }
        bool FoundPlayer()
        {
            var colliders = Physics.OverlapSphere(transform.position, sightRadius);
            foreach (var target in colliders)
            {
                if (target.CompareTag("Player"))
                {
                    attackTarget = target.gameObject;
                    return true;
                }
            }
            attackTarget = null;
            return false;
        }
        bool TargetInAttackRange()
        {
            if (attackTarget != null)
                return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
            else
                return false;
        }
        bool TargetInSkillRange()
        {
            if (attackTarget != null)
                return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
            else
                return false;
        }
        void GetNewWayPoint()
        {
            remainLookAtTime = lookAtTime;
            float randomX = Random.Range(-patrolRange, patrolRange);
            float randomZ = Random.Range(-patrolRange, patrolRange);

            Vector3 randomPoint = new Vector3(guardPos.x + randomX,transform.position.y, guardPos.z + randomZ);
            NavMeshHit hit;
            wayPoint=NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        }
        void OnDrawGizmosSelected()
        {
            //显示巡逻范围
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sightRadius);
        }
        //玩家受伤
        void Hit()
        {
            if (attackTarget != null)
            {
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeDamage(characterStats, targetStats);
                if (characterStats.isCritical)
                {
                    targetStats.GetComponentInChildren<Animator>().SetTrigger("Hit");
                }
            }
        }
        //敌人受伤
        private void OnTriggerEnter(Collider col)
        {
            print(col.name);
            var targetDefence = GetComponent<CharacterStats>();
            var targetAttack = player.GetComponent<CharacterStats>();
            targetDefence.TakeDamage(targetAttack, targetDefence);
            targetDefence.GetComponent<Animator>().SetTrigger("EnemyA_Hit");
        }

        public void EndNotify()
        {
            //玩家死亡后
            playerIsDead = true;
            isChase = false;
            isWalk = false;
            attackTarget = null;

        }
    }
}
