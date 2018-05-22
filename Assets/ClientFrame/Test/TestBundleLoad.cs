using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U3dClient;

public class TestBundleLoad : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		var isLoaded = false;
		GameRoot.Instance.ResourceMgr.InitResourceLoader(() =>
		{
		    isLoaded = true;
		});
		while  (!isLoaded)
		{
			yield return null;
		}
		Debug.Log("=============");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
