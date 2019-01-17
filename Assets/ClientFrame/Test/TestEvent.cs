using System.Collections;
using System.Collections.Generic;
using U3dClient;
using UnityEngine;

public class TestEvent : MonoBehaviour
{

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 180, 180), "注册事件"))
        {
            Test1();
        }

        if (GUI.Button(new Rect(20, 250, 180, 180), "发送事件"))
        {
            Test2();
        }

        if (GUI.Button(new Rect(20, 250 + 210, 180, 180), "取消事件"))
        {
            Test3();
        }
    }

    private static int eventIndex1;
    private static int eventIndex2;
    private static int eventIndex3;
    private static int eventIndex4;

    private void Test1()
    {
        eventIndex1 = GameCenter.s_EventManager.RegisterEvent("Test1", message =>
        {
            Debug.Log($"Test1---------1  {eventIndex2}"); GameCenter.s_EventManager.UnRegisterEvent(eventIndex2);
        });
        eventIndex2 = GameCenter.s_EventManager.RegisterEvent("Test1", message =>
        {
            Debug.Log($"Test1---------2 {eventIndex1}"); GameCenter.s_EventManager.UnRegisterEvent(eventIndex1);
        });
        eventIndex3 = GameCenter.s_EventManager.RegisterEvent("Test2", message => { Debug.Log("Test2---------1"); });
        eventIndex4 = GameCenter.s_EventManager.RegisterEvent("Test2", message => { Debug.Log("Test2---------2"); });
    }

    private void Test2()
    {
        GameCenter.s_EventManager.FireEvent("Test1", isImmediate:false);
        //        GameCenter.s_EventManager.FireEvent("Test2");
        var message = EventMessage<int>.GetEventMessage();
        message.Value1 = 200;
        //        GameCenter.s_EventManager.FireEvent("Test1", message);
    }

    private void Test3()
    {
    }
}
