using System.Collections;
using System.Collections.Generic;
using U3dClient.Game;
using UnityEngine;

namespace U3dClient.Frame
{
    [DefaultExecutionOrder(-1000)]
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

           GameFrameCenter.Awake();
           GameLogicCenter.Awake();
        }

        private void Start()
        {
            GameFrameCenter.Start();
            GameLogicCenter.Start();
        }

        private void Update()
        {
            GameFrameCenter.Update();
            GameLogicCenter.Update();
        }

        private void FixedUpdate()
        {
            GameFrameCenter.FixedUpdate();
            GameLogicCenter.FixedUpdate();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            GameFrameCenter.OnApplicationFocus(hasFocus);
            GameLogicCenter.OnApplicationFocus(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            GameFrameCenter.OnApplicationPause(pauseStatus);
            GameLogicCenter.OnApplicationPause(pauseStatus);
        }

        private void OnDestroy()
        {
            GameFrameCenter.OnDestroy();
            GameLogicCenter.OnDestroy();
        }

        private void OnApplicationQuit()
        {
            GameFrameCenter.OnApplicationQuit();
            GameLogicCenter.OnApplicationQuit();
        }
    }
}

