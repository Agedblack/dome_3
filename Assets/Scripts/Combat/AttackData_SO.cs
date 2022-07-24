using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulLike
{
    [CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
    public class AttackData_SO : ScriptableObject
    {
        public float attackRange;//攻击距离
        public float skillRange;//特殊攻击距离
        public float coolDown;//cd
        public int minDamge;//最小攻击数值
        public int maxDamge;//最大攻击数值
        public float criticalMultiplier;//暴击伤害
        public float criticalChance;//暴击率

    }
}
