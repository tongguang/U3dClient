using System.Collections;
using System.Collections.Generic;
using U3dClient;
using UnityEngine;

public class TestTimer : MonoBehaviour {

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 180, 180), "开始定时器"))
        {
            Test1();
        }

        if (GUI.Button(new Rect(20, 250, 180, 180), "停止定时器"))
        {
            Test2();
        }

//        if (GUI.Button(new Rect(20, 250 + 210, 180, 180), "卸载"))
//        {
//            Test3();
//        }
    }

    private static int timer;
    private void Test1()
    {
        timer = GameCenter.s_TimerManager.RegisterTimer(1, () => { Debug.Log("complate"); }, f => { Debug.Log(f); }, true);
    }

    private void Test2()
    {
        GameCenter.s_TimerManager.CancelTimer(timer);
    }

    private void Test3()
    {
    }
}
