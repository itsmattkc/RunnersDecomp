using System.Collections;
using UnityEngine;

public class AssetBundleLoadSceneTest : MonoBehaviour
{
	private void Start()
	{
		WWW www = WWW.LoadFromCacheOrDownload("http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/ResourcesCommonPrefabs.unity3d", 5);
		StartCoroutine(WaitLoard(www, "ResourcesCommonPrefabs"));
		WWW www2 = WWW.LoadFromCacheOrDownload("http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/ResourcesCommonObject.unity3d", 5);
		StartCoroutine(WaitLoard(www2, "ResourcesCommonObject"));
	}

	private void Update()
	{
	}

	private IEnumerator WaitLoard(WWW www, string scenename)
	{
		while (www == null)
		{
			yield return null;
		}
		if (www.error != null || www.error != null)
		{
			Debug.LogError(www.error);
		}
		else
		{
			Application.LoadLevelAdditive(scenename);
		}
	}
}
