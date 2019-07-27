using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U3dClient;

public class TestLoadAsset : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 180, 180), "下载"))
        {
            Test1();
        }

        if (GUI.Button(new Rect(20, 250, 180, 180), "加载"))
        {
            Test2();
        }

        if (GUI.Button(new Rect(20, 250 + 210, 180, 180), "卸载"))
        {
            Test3();
        }
    }

    private void Test1()
    {
        GameCenter.s_UpgradeManager.StartUpdate(() => { Debug.Log("下载结束"); },
            (updated, total) => { Debug.Log(string.Format("下载进度 {0} {1}", updated, total)); });
    }

    private static int refIndex;

    private void Test2()
    {
        refIndex = GameCenter.s_ResourceManager.LoadAssetAsync<GameObject>("res/test2.ab", "Image.prefab",
            (isOk, o) => { Instantiate(o, transform); });
    }

    private void Test3()
    {
        BundleAssetLoader.UnLoad(refIndex);
    }
}