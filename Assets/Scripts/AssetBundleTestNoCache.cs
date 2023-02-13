using System.Collections;
using UnityEngine;

public class AssetBundleTestNoCache : MonoBehaviour
{
	private void Start()
	{
		WWW www = new WWW("http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/PrephabKnuckles.unity3d");
		StartCoroutine(WaitLoard(www));
	}

	private void Update()
	{
	}

	private IEnumerator WaitLoard(WWW www)
	{
		while (!www.isDone)
		{
			yield return null;
		}
		if (www.error != null)
		{
			Debug.LogError(www.error);
			yield break;
		}
		AssetBundle myLoadedAssetBundle = www.assetBundle;
		Object asset = myLoadedAssetBundle.mainAsset;
		Object.Instantiate(asset);
		myLoadedAssetBundle.Unload(false);
	}
}
