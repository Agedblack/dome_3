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

        [Header("׷�����")]
        public float sightRadius;
        private GameObject attackTarget;
        public bool isGuard;
        private float speed_EnemyA;
        private float lastAttackTime;//�������

        bool isWalk;
        bool isChase;
        bool isDead;
        public bool playerIsDead;
        [Header("Ѳ�߷�Χ")]
        public float patrolRange;
        public float lookAtTime;//Ѳ�߹۲�ʱ��
        private float remainLookAtTime;//����۲��ʱ��
        private Vector3 wayPoint;//�����
        private Vector3 guardPos;//��ʼ��

        private Quaternion guardRotation;//��¼��ת�Ƕ�
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
        //    //����
        //    GameManager.Instance.AddObserver(this);
        //}
        void OnDisable()
        {
            //����
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
            //�������player �л���׷��
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
                    //�ж��Ƿ񵽴����Ѳ�ߵ�
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
                    //TODO:׷Player
                    //TODO���ڹ�����Χ���򹥻�
                    //TODO����϶���
                    isWalk = false;
                    isChase = true;
                    agent.speed = speed_EnemyA;
                    if (!FoundPlayer())
                    {
                        //TODO�����ѻص���һ��״̬
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
                    //TODO:�ڹ�����Χ���򹥻�
                    if (TargetInAttackRange() || TargetInSkillRange())
                    {
                        isChase = false;
                        agent.isStopped = true;
                        if (lastAttackTime < 0)
                        {
                            //�������
                            lastAttackTime = characterStats.attackData.coolDown;
                            //����
                            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                            //ִ�й���
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
                //��ͨ����
                anim.SetTrigger("EnemyA_Attack");
            }
            if (TargetInSkillRange())
            {
                //���⹥��
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
            //��ʾѲ�߷�Χ
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sightRadius);
        }
        //�������
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
        //��������
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
            //���������
            playerIsDead = true;
            isChase = false;
            isWalk = false;
            attackTarget = null;

        }
    }
}
