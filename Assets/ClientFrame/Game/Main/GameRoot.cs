using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.Game
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("GameRoot 初始化两次");
                return;
            }

           GameCenter.Awake();
        }

        private void Start()
        {
            GameCenter.Start();
        }

        private void Update()
        {
            GameCenter.Update();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            GameCenter.OnApplicationFocus(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            GameCenter.OnApplicationPause(pauseStatus);
        }

        private void OnDestroy()
        {
            GameCenter.OnDestroy();
        }

        private void OnApplicationQuit()
        {
            GameCenter.OnApplicationQuit();
        }
    }
}

