using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulLike
{
    [CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
    public class AttackData_SO : ScriptableObject
    {
        public float attackRange;//��������
        public float skillRange;//���⹥������
        public float coolDown;//cd
        public int minDamge;//��С������ֵ
        public int maxDamge;//��󹥻���ֵ
        public float criticalMultiplier;//�����˺�
        public float criticalChance;//������

    }
}
