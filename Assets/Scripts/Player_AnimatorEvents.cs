using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulLike
{
    public class Player_AnimatorEvents : MonoBehaviour
    {
        PlayerController playerController;
        Animator anim_Player;
        private void Awake()
        {
            anim_Player = GetComponent<Animator>();
            playerController= GetComponentInParent<PlayerController>();
        }
        private void OnAnimatorMove()
        {
            SendMessageUpwards("AnimatorRootMotion", anim_Player.deltaPosition);//模型移动量
        }
        public void PlayerAnimOver()
        {
            playerController.inputEnablad = true;
        }
        public void PlayerAnimStart()
        {
            playerController.inputEnablad = false;
        }
    }
}
