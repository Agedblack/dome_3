using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulLike
{
    public class GameManager :Singleton<GameManager>
    {
        public CharacterStats playerStats;
        List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
        public void RigisterPlayer(CharacterStats player)
        {
            playerStats = player;
        }

        public void AddObserver(IEndGameObserver observer)
        {
            endGameObservers.Add(observer);
        }
        public void RemoveObserver(IEndGameObserver observer)
        {
            endGameObservers.Remove(observer);
        }
        public void NotifyObserVers()
        {
            foreach (var observer in endGameObservers)
            {
                observer.EndNotify();
            }
        }
    }
}