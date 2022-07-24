using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulLike
{
    [CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
    public class CharacterData_SO : ScriptableObject
    {
        [Header("ÊôÐÔ")]
        public int maxHealth;
        public int currentHealth;
        public int baseDefence;
        public int currentDefence;
        [Header("¾­Ñé")]
        public int currentLevel;
        public int maxLevel;
        public int baseExp;
        public int currentExp;
        public float levelBuff;
        [Header("»÷É±")]
        public int killPoint;

        public float LeveMultiplier
        {
            get { return 1 + (currentLevel - 1) * levelBuff; }
        }
        public void UpdateExp(int point)
        {
            currentExp += point;
            if (currentExp >= baseExp)
                LeveUp();
        }

        private void LeveUp()
        {
            currentLevel = Mathf.Clamp(currentLevel + 1,0,maxLevel);
            baseExp += (int)(baseExp * LeveMultiplier);

            maxHealth = (int)(maxHealth * LeveMultiplier);
            currentHealth = maxHealth;

            Debug.Log("LEVEL UP" + currentLevel + "Max Health:" + maxHealth);
        }
    }
}
