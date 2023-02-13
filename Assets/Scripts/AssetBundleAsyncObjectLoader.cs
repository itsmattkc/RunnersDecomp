using System.Collections;
using UnityEngine;

public class AssetBundleAsyncObjectLoader : MonoBehaviour
{
	private UnityEngine.AssetBundleRequest mAbRquest;

	private AsyncLoadedObjectCallback mAsyncLoadedCallback;

	private bool mLoading;

	public AsyncLoadedObjectCallback asyncLoadedCallback
	{
		get
		{
			return mAsyncLoadedCallback;
		}
		set
		{
			mAsyncLoadedCallback = value;
		}
	}

	public UnityEngine.AssetBundleRequest assetBundleRequest
	{
		get
		{
			return mAbRquest;
		}
		set
		{
			mAbRquest = value;
		}
	}

	private void Awake()
	{
		mLoading = true;
	}

	private void Start()
	{
		StartCoroutine("Watch");
	}

	private void Update()
	{
		if (!mLoading)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator Watch()
	{
		yield return mAbRquest;
		mAsyncLoadedCallback(mAbRquest.asset);
		mLoading = false;
	}
}
