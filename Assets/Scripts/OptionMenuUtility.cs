using UnityEngine;

public class OptionMenuUtility
{
	private const string ATTACH_ANTHOR_NAME = "UI Root (2D)/Camera/Anchor_5_MC";

	public static GameObject CreateSceneLoader(string sceneName)
	{
		GameObject gameObject = new GameObject("SceneLoader");
		if (gameObject != null)
		{
			ResourceSceneLoader resourceSceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
			if (resourceSceneLoader != null)
			{
				bool onAssetBundle = true;
				resourceSceneLoader.AddLoad(sceneName, onAssetBundle, false);
			}
		}
		return gameObject;
	}

	public static GameObject CreateSceneLoader(string sceneName, bool assetBundleFlag)
	{
		GameObject gameObject = new GameObject("SceneLoader");
		if (gameObject != null)
		{
			ResourceSceneLoader resourceSceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
			if (resourceSceneLoader != null)
			{
				resourceSceneLoader.AddLoad(sceneName, assetBundleFlag, false);
			}
		}
		return gameObject;
	}

	public static void DestroySceneLoader(GameObject obj)
	{
		Object.Destroy(obj);
		obj = null;
	}

	public static bool CheckSceneLoad(GameObject obj)
	{
		if (obj != null)
		{
			ResourceSceneLoader component = obj.GetComponent<ResourceSceneLoader>();
			if (component != null)
			{
				return component.Loaded;
			}
		}
		return true;
	}

	public static void TranObj(GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find("UI Root (2D)/Camera/Anchor_5_MC");
		if (gameObject != null)
		{
			Transform transform = obj.transform;
			Transform transform2 = gameObject.transform;
			if (transform != null && transform2 != null)
			{
				Vector3 localScale = transform.transform.localScale;
				transform.parent = transform2;
				transform.transform.localPosition = new Vector3(0f, 0f, 0f);
				transform.transform.localScale = localScale;
			}
		}
	}
}
