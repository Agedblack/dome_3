using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulLike
{
    public class Player_AttackEvents : MonoBehaviour
    {
        PlayerController playerController;
        public CapsuleCollider WCap;
        private void Awake()
        {
            playerController = GetComponentInParent<PlayerController>();
        }
        public void ComboReset()
        {
            playerController.comboPossibleL = false;
            playerController.comboPossibleR = false;
            playerController.comboStep = 0;
            playerController.inputEnablad=true;
        }
        public void Player_AttackL()
        {
            playerController.comboPossibleL = true;
        }
        public void Player_AttackR()
        {
            playerController.comboPossibleR = true;
        }
        public void Player_AttackLOver()
        {
            playerController.comboL = false;
            playerController.inputEnablad = true;
            playerController.attackOverB = true;
        }
        public void Player_AttackROver()
        {
            playerController.comboR = false;
            playerController.inputEnablad = true;
            playerController.attackOverB = true;
        }
        public void Player_Trigger_true()
        {
            WCap.isTrigger = true;
        }
        public void Player_Trigger_false()
        {
            WCap.isTrigger = false;
        }
    }
}
