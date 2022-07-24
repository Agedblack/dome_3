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
                    //同场景
                    StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));//启动协程
                    break;
                case TransitionPoint.TransitionType.DifferentScene:
                    //不同场景
                    StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                    break;
                default:
                    break;
            }
        }

        IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
        {
            //SaveManager.Instance.SavePlayerData();//储存
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                //不同场景
                yield return SceneManager.LoadSceneAsync(sceneName);
                yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
                //SaveManager.Instance.LoadPlayerData();//读取
                yield break;
            }
            else
            {
                //同场景
                player = GameManager.Instance.playerStats.gameObject;//拿到player
                player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);//拿到终点和方向
                yield return null;
            }
        }

        private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
        {
            var entrances = FindObjectsOfType<TransitionDestination>();//获取所有的目标点

            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].destinationTag == destinationTag)//如果标签一样
                    return entrances[i];
            }
            return null;//默认状态返回空
        }
    }
}
