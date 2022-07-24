using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulLike
{
    public class TransitionPoint : MonoBehaviour
    {
        public enum TransitionType
        {
            SameScene, DifferentScene//同场景 不同场景
        }
        [Header("传送")]
        public string sceneName;
        public TransitionType transitionType;//场景

        public TransitionDestination.DestinationTag destinationTag;//终点
        private bool canTrans;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && canTrans)
            {
                //传送
                SceneController.Instance.TransitionToDestination(this);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
                canTrans = true;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                canTrans = false;
        }
    }
}
