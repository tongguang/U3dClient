using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U3dClient;

public class TestBundleLoad : MonoBehaviour {

	// Use this for initialization
//    public GameObject Parent1;
//	IEnumerator Start () {
//        var refIndex = GameRoot.Instance.ResourceMgr.ResourceLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
//	        o =>
//	        {
//	            Instantiate(o, Parent1.transform);
//	        }, true);
//	    yield return new WaitForSeconds(5);
//        var refIndex2 = GameRoot.Instance.ResourceMgr.ResourceLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
//            o =>
//            {
//                Instantiate(o, Parent1.transform);
//            }, true);
//		Debug.Log("===============11");
//        GameRoot.Instance.ResourceMgr.ResourceLoader.UnLoadAsset(refIndex);
//        GameRoot.Instance.ResourceMgr.ResourceLoader.UnLoadAsset(refIndex2);
//    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 80, 20), "下载"))
        {
            Test1();
        }
        if (GUI.Button(new Rect(20, 70, 80, 20), "加载"))
        {
            Test2();
        }
    }

    private void Test1()
    {
        GameRoot.Instance.UpdateMgr.StartUpdate(() => { Debug.Log("下载结束"); }, (updated, total) => {});
    }

    private void Test2()
    {
        var refIndex = GameRoot.Instance.ResourceMgr.ResourceLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
	        o =>
	        {
	            Instantiate(o, transform);
	        }, true);
        var refIndex2 = GameRoot.Instance.ResourceMgr.ResourceLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
            o =>
            {
                var go = Instantiate(o, transform);
                ((RectTransform) go.transform).anchoredPosition = new Vector2(50, 50);
            }, true);
        //        GameRoot.Instance.ResourceMgr.ResourceLoader.UnLoadAsset(refIndex);
        //        GameRoot.Instance.ResourceMgr.ResourceLoader.UnLoadAsset(refIndex2);
    }
}
