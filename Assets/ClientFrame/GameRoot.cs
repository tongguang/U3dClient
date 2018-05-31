﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance;

        public ResourceManager ResourceMgr = new ResourceManager();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            ResourceMgr.Awake();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

