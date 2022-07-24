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
            SameScene, DifferentScene//ͬ���� ��ͬ����
        }
        [Header("����")]
        public string sceneName;
        public TransitionType transitionType;//����

        public TransitionDestination.DestinationTag destinationTag;//�յ�
        private bool canTrans;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && canTrans)
            {
                //����
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
