using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulLike
{
    public class SceneController :Singleton<SceneController>
    {
        public GameObject player;
        public GameObject playerPrefab;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
        public void TransitionToDestination(TransitionPoint transitionPoint)
        {
            switch (transitionPoint.transitionType)
            {
                case TransitionPoint.TransitionType.SameScene:
                    //ͬ����
                    StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));//����Э��
                    break;
                case TransitionPoint.TransitionType.DifferentScene:
                    //��ͬ����
                    StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                    break;
                default:
                    break;
            }
        }

        IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
        {
            //SaveManager.Instance.SavePlayerData();//����
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                //��ͬ����
                yield return SceneManager.LoadSceneAsync(sceneName);
                yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
                //SaveManager.Instance.LoadPlayerData();//��ȡ
                yield break;
            }
            else
            {
                //ͬ����
                player = GameManager.Instance.playerStats.gameObject;//�õ�player
                player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);//�õ��յ�ͷ���
                yield return null;
            }
        }

        private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
        {
            var entrances = FindObjectsOfType<TransitionDestination>();//��ȡ���е�Ŀ���

            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].destinationTag == destinationTag)//�����ǩһ��
                    return entrances[i];
            }
            return null;//Ĭ��״̬���ؿ�
        }
    }
}
