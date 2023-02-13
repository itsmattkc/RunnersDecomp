using System;
using System.Collections;
using UnityEngine;

public class AssetBundleAsyncDownloader : MonoBehaviour
{
	private AssetBundleRequest mAbRquest;

	private AsyncDownloadCallback mAsyncDownloadCallback;

	private bool mDownloading;

	public AsyncDownloadCallback asyncLoadedCallback
	{
		get
		{
			return mAsyncDownloadCallback;
		}
		set
		{
			mAsyncDownloadCallback = value;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		if (mAbRquest != null)
		{
			try
			{
				StartCoroutine(Load());
			}
			catch (Exception ex)
			{
				Debug.Log("AssetBundleAsyncDownloader.Start() ExceptionMessage = " + ex.Message + "ToString() = " + ex.ToString());
			}
		}
	}

	public void SetBundleRequest(AssetBundleRequest request)
	{
		mAbRquest = request;
	}

	private void Update()
	{
	}

	private IEnumerator Load()
	{
		WWW www2 = null;
		www2 = ((!mAbRquest.useCache) ? new WWW(mAbRquest.path) : ((mAbRquest.crc == 0) ? WWW.LoadFromCacheOrDownload(mAbRquest.path, mAbRquest.version) : WWW.LoadFromCacheOrDownload(mAbRquest.path, mAbRquest.version, mAbRquest.crc)));
		yield return www2;
		if (mAsyncDownloadCallback != null)
		{
			mAsyncDownloadCallback(www2);
		}
	}
}
