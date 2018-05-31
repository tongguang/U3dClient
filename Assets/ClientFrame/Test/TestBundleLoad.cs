using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U3dClient;

public class TestBundleLoad : MonoBehaviour {

	// Use this for initialization
    public GameObject Parent1;
	IEnumerator Start () {
//        var isLoaded = false;
//        GameRoot.Instance.ResourceMgr.ResourceLoader.InitAsync(() =>
//        {
//            isLoaded = true;
//        });
//        while (!isLoaded)
//        {
//            yield return null;
//        }

        GameRoot.Instance.ResourceMgr.ResourceLoader.Init();
        var refIndex = GameRoot.Instance.ResourceMgr.ResourceLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
	        o =>
	        {
	            Instantiate(o, Parent1.transform);
	        }, true);
	    yield return new WaitForSeconds(5);
        var refIndex2 = GameRoot.Instance.ResourceMgr.ResourceLoader.LoadAsset<GameObject>("res/test2.ab", "Image",
            o =>
            {
                Instantiate(o, Parent1.transform);
            }, true);
		Debug.Log("===============11");
        GameRoot.Instance.ResourceMgr.ResourceLoader.UnLoadAsset(refIndex);
        GameRoot.Instance.ResourceMgr.ResourceLoader.UnLoadAsset(refIndex2);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
