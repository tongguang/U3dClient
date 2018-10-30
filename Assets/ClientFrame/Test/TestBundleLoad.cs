using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U3dClient;
using U3dClient.ResourceMgr;
using U3dClient.UpdateMgr;

public class TestBundleLoad : MonoBehaviour
{
    // Use this for initialization
//    public GameObject Parent1;
//	IEnumerator Start () {
//        var refIndex =  GameCenter.ResourceMgr.BundleLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
//	        o =>
//	        {
//	            Instantiate(o, Parent1.transform);
//	        }, true);
//	    yield return new WaitForSeconds(5);
//        var refIndex2 =  GameCenter.ResourceMgr.BundleLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
//            o =>
//            {
//                Instantiate(o, Parent1.transform);
//            }, true);
//		Debug.Log("===============11");
//         GameCenter.ResourceMgr.BundleLoader.UnLoadAsset(refIndex);
//         GameCenter.ResourceMgr.BundleLoader.UnLoadAsset(refIndex2);
//    }

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
        UpdateManager.StartUpdate(() => { Debug.Log("下载结束"); },
            (updated, total) => { Debug.Log(string.Format("下载进度 {0} {1}", updated, total)); });
    }

    private static int refIndex;

    private void Test2()
    {
        refIndex = BundleAssetBaseLoader.LoadAsync<GameObject>("res/test2.ab", "Image",
            (isOk, o) => { Instantiate(o, transform); });
//        var refIndex2 = BundleAssetBaseLoader.SLoadAsync<GameObject>("res/test2.ab", "Image",
//            (isOk, o) =>
//            {
//                var go = Instantiate(o, transform);
//                ((RectTransform) go.transform).anchoredPosition = new Vector2(50, 50);
//            });
//        BundleAssetBaseLoader.SUnLoad(refIndex);
//        BundleAssetBaseLoader.SUnLoad(refIndex2);
    }

    private void Test3()
    {
        BundleAssetBaseLoader.UnLoad(refIndex);
    }
}