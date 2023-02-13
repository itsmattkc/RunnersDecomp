using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleResult
{
	private AssetBundle mAssetBundle;

	private GameObject mObjects;

	private AssetBundleAsyncObjectLoader mAbAsyncLoader;

	private List<AsyncLoadedObjectCallback> mAbAsyncLoaderCallback;

	private Texture2D mTexture;

	private string mText;

	private byte[] mBytes;

	private string mPath;

	private string mName;

	public string mError;

	private bool mIsValid;

	public Texture2D Texture
	{
		get
		{
			return mTexture;
		}
	}

	public string Text
	{
		get
		{
			return mText;
		}
	}

	public byte[] bytes
	{
		get
		{
			return mBytes;
		}
	}

	public string Error
	{
		get
		{
			return mError;
		}
	}

	public string Path
	{
		get
		{
			return mPath;
		}
	}

	public string Name
	{
		get
		{
			return mName;
		}
	}

	public bool isValid
	{
		get
		{
			return mIsValid;
		}
		set
		{
			mIsValid = value;
		}
	}

	public AssetBundleResult(string path, AssetBundle assetBundle, string err)
	{
		Initialize(path, assetBundle, null, null, null, err);
		mAbAsyncLoaderCallback = new List<AsyncLoadedObjectCallback>(2);
	}

	public AssetBundleResult(string path, AssetBundle assetBundle, Texture2D texture, string err)
	{
		Initialize(path, assetBundle, texture, null, null, err);
	}

	public AssetBundleResult(string path, AssetBundle assetBundle, TextAsset textAsset, string err)
	{
		Initialize(path, assetBundle, null, textAsset.text, null, err);
	}

	public AssetBundleResult(string path, AssetBundle assetBundle, string text, string err)
	{
		Initialize(path, assetBundle, null, text, null, err);
	}

	public AssetBundleResult(string path, byte[] bytes, string err)
	{
		Initialize(path, null, null, null, bytes, err);
	}

	public void Initialize(string path, AssetBundle assetBundle, Texture2D texture, string text, byte[] bytes, string err)
	{
		mAssetBundle = assetBundle;
		mAbAsyncLoader = null;
		mAbAsyncLoaderCallback = null;
		mObjects = null;
		mTexture = texture;
		mText = text;
		mBytes = bytes;
		mError = err;
		mPath = path;
		mName = System.IO.Path.GetFileNameWithoutExtension(path);
		mIsValid = true;
	}

	public GameObject LoadObject(string objectName)
	{
		if (null == mObjects)
		{
			if (null == mAssetBundle)
			{
				Debug.LogError("AssetBundleResult : LoadObject : mAssetBundle is null");
			}
			else
			{
				mObjects = (mAssetBundle.Load(objectName, typeof(GameObject)) as GameObject);
				mAssetBundle.Unload(false);
				mAssetBundle = null;
			}
		}
		return mObjects;
	}

	public void LoadGameObjectAsync(string objectName, AsyncLoadedObjectCallback callback)
	{
		if (null == mObjects)
		{
			if (null == mAssetBundle)
			{
				Debug.LogError("AssetBundleResult : LoadObject : mAssetBundle is null");
				return;
			}
			mAbAsyncLoaderCallback.Add(callback);
			if (null == mAbAsyncLoader)
			{
				GameObject gameObject = new GameObject("async load object");
				mAbAsyncLoader = gameObject.AddComponent<AssetBundleAsyncObjectLoader>();
				mAbAsyncLoader.assetBundleRequest = mAssetBundle.LoadAsync(objectName, typeof(GameObject));
				mAbAsyncLoader.asyncLoadedCallback = AsyncLoadCallback;
			}
		}
		else if (callback != null)
		{
			callback(mObjects);
		}
	}

	private void AsyncLoadCallback(Object loadedObject)
	{
		mObjects = (loadedObject as GameObject);
		int count = mAbAsyncLoaderCallback.Count;
		for (int i = 0; i < count; i++)
		{
			AsyncLoadedObjectCallback asyncLoadedObjectCallback = mAbAsyncLoaderCallback[i];
			if (asyncLoadedObjectCallback != null)
			{
				asyncLoadedObjectCallback(mObjects);
			}
		}
		mAbAsyncLoaderCallback.Clear();
		mAbAsyncLoaderCallback = null;
		mAssetBundle.Unload(false);
		mAssetBundle = null;
		mAbAsyncLoader = null;
	}

	public void Clear()
	{
		if (null != mAbAsyncLoader)
		{
			mAbAsyncLoader.asyncLoadedCallback = null;
		}
		bool flag = false;
		if (null != mAssetBundle)
		{
			flag = true;
			mAssetBundle.Unload(false);
		}
		if (null != mObjects)
		{
			Object.DestroyImmediate(mObjects, true);
		}
		if (!flag && null != mTexture)
		{
			Object.Destroy(mTexture);
		}
		mAbAsyncLoader = null;
		mAssetBundle = null;
		mObjects = null;
		mTexture = null;
		mText = null;
		mBytes = null;
		mPath = null;
		mError = null;
		mIsValid = false;
	}
}
