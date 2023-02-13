using System.Collections;
using UnityEngine;

public class AssetBundleTest : MonoBehaviour
{
	private void Start()
	{
		WWW www = WWW.LoadFromCacheOrDownload("http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/PrephabKnuckles.unity3d", 5);
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
