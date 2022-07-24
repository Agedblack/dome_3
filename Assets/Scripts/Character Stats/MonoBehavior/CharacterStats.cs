using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulLike
{
    public class CharacterStats : MonoBehaviour
    {
        public event Action<int, int> UpdateHealthBarOnAttack;
        public CharacterData_SO templateData;
        public CharacterData_SO characterData;
        public AttackData_SO attackData;
        [HideInInspector]
        public bool isCritical;//是否暴击
        private void Awake()
        {
            if (templateData != null)
                characterData = Instantiate(templateData);
        }
        #region 人物属性
        public int MaxHealth
        {
            get { if (characterData != null) return characterData.maxHealth; else return 0; }
            set { characterData.maxHealth = value; }
        }
        public int CurrentHealth
        {
            get { if (characterData != null) return characterData.currentHealth; else return 0; }
            set { characterData.currentHealth = value; }
        }
        public int BaseDefence
        {
            get { if (characterData != null) return characterData.baseDefence; else return 0; }
            set { characterData.baseDefence = value; }
        }
        public int CurrentDefence
        {
            get { if (characterData != null) return characterData.currentDefence; else return 0; }
            set { characterData.currentDefence = value; }
        }
        #endregion
        #region 受击
        public void TakeDamage(CharacterStats attacker,CharacterStats defener)
        {
            int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,2);
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
            Debug.Log(damage);
            //UI
            UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
            //经验
            if (defener.CurrentHealth <= 0)
            {
                attacker.characterData.UpdateExp(defener.characterData.killPoint);
            }
        }

        private int CurrentDamage()
        {
            float coreDamage = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamge);
            if (isCritical)
            {
                coreDamage *= attackData.criticalMultiplier;
                Debug.Log("暴击" + coreDamage);
            }
            return (int)coreDamage;
        }
        #endregion
    }
}
