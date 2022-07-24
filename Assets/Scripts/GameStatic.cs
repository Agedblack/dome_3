using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulLike
{
    public static class GameStatic
    {
        /// <summary>
        /// 斜方向向量控制为1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector2 SquareToCircle(Vector2 input)
        {
            Vector2 output = Vector2.zero;
            output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
            output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
            return output;
        }
        /// <summary>
        /// 检测是否在某个Base Layer中的状态
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool CheckState(Animator animator, string stateName, string layerName = "Base Layer")
        {
            return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsName(stateName);
        }
        /// <summary>
        /// 检测是否在某个Base Layer中的tag
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="tagName"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool CheckStateTag(Animator animator, string tagName, string layerName = "Base Layer")
        {
            return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsTag(tagName);
        }
    }
}
