using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	private class WaitInfo
	{
		public AssetBundleRequest mRequest;
	}

	private enum CancelState
	{
		IDLE,
		CANCELING,
		CANCELED
	}

	private Dictionary<string, AssetBundleRequest> mAssetDic = new Dictionary<string, AssetBundleRequest>();

	private static AssetBundleManager mInstance;

	private bool mExecuting;

	private List<AssetBundleRequest> mRequestList = new List<AssetBundleRequest>();

	private List<AssetBundleRequest> mExecuteList = new List<AssetBundleRequest>();

	private List<AssetBundleRequest> mRemainingList = new List<AssetBundleRequest>();

	private List<string> mReqUnloadList = new List<string>();

	public int mAllLoadedAssetCount;

	public int mLoadedTextureAssetCount;

	private CancelState mCancelState;

	private bool mCancelRequest;

	public int Count
	{
		get
		{
			return mAssetDic.Count;
		}
	}

	public static AssetBundleManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	public int RequestCount
	{
		get
		{
			return mRequestList.Count;
		}
	}

	public bool Executing
	{
		get
		{
			return mExecuting;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	private void OnDestroy()
	{
		if (mInstance == this)
		{
			mInstance = null;
		}
	}

	protected bool CheckInstance()
	{
		if (mInstance == null)
		{
			mInstance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(this);
		return false;
	}

	public static void Create()
	{
		if (mInstance == null)
		{
			GameObject gameObject = new GameObject("AssetBundleManager");
			gameObject.AddComponent<AssetBundleManager>();
			Debug.Log("AssetBundleManager.Create", DebugTraceManager.TraceType.ASSETBUNDLE);
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		if (mReqUnloadList.Count > 0)
		{
			foreach (string mReqUnload in mReqUnloadList)
			{
				Unload(mReqUnload);
			}
			mReqUnloadList.Clear();
		}
		if (mExecuting)
		{
			return;
		}
		int count = mRequestList.Count;
		if (0 < count)
		{
			mExecuteList.Clear();
			mRemainingList.Clear();
			for (int i = 0; i < count; i++)
			{
				mExecuteList.Add(mRequestList[i]);
				mRemainingList.Add(mRequestList[i]);
			}
			mRequestList.Clear();
			mExecuting = true;
			StartCoroutine("ExecuteRequest");
		}
	}

	public AssetBundleRequest RequestNoCache(string path, AssetBundleRequest.Type type, GameObject returnObject)
	{
		return Request(path, 0, 0u, type, returnObject, false);
	}

	public AssetBundleRequest Request(string path, int version, uint crc, AssetBundleRequest.Type type, GameObject returnObject, bool useCache)
	{
		if (IsCancelRequest())
		{
			return null;
		}
		AssetBundleRequest value = null;
		if (mAssetDic.TryGetValue(path, out value))
		{
			Debug.Log("AssetBundleManager.Request : already exist : " + path, DebugTraceManager.TraceType.ASSETBUNDLE);
			value.Result();
			if (returnObject != null)
			{
				MsgAssetBundleResponseSucceed value2 = new MsgAssetBundleResponseSucceed(value, value.assetbundleResult);
				value.returnObject.SendMessage("AssetBundleResponseSucceed", value2, SendMessageOptions.DontRequireReceiver);
			}
			return value;
		}
		value = new AssetBundleRequest(path, version, crc, type, returnObject, useCache);
		value.TimeOut = AssetBundleRequest.DefaultTimeOut;
		mAssetDic[value.path] = value;
		mRequestList.Add(value);
		return value;
	}

	public AssetBundleRequest ReRequest(AssetBundleRequest request)
	{
		if (IsCancelRequest())
		{
			return null;
		}
		AssetBundleRequest assetBundleRequest = new AssetBundleRequest(request);
		request.TimeOut += AssetBundleRequest.DefaultTimeOut;
		mAssetDic[request.path] = assetBundleRequest;
		mRequestList.Add(assetBundleRequest);
		return assetBundleRequest;
	}

	public AssetBundleRequest GetResource(string path)
	{
		AssetBundleRequest value = null;
		if (mAssetDic.TryGetValue(path, out value))
		{
			return value;
		}
		return null;
	}

	public void Unload(string url)
	{
		AssetBundleRequest value = null;
		bool flag = false;
		if (mAssetDic.TryGetValue(url, out value))
		{
			value.Unload();
			if (mAssetDic.Remove(url))
			{
				flag = true;
			}
		}
		if (flag)
		{
			Debug.Log("AssetBundleManager.Unload() : " + url, DebugTraceManager.TraceType.ASSETBUNDLE);
		}
		else
		{
			Debug.LogWarning("AssetBundleManager.Unload : But [" + url + "] is not exist", DebugTraceManager.TraceType.ASSETBUNDLE);
		}
	}

	public void UnloadWithCancel(string url)
	{
		AssetBundleRequest value = null;
		if (mAssetDic.TryGetValue(url, out value))
		{
			if (value.IsLoading())
			{
				value.Cancel();
			}
			else
			{
				value.Unload();
			}
			if (!mAssetDic.Remove(url))
			{
			}
		}
	}

	public void RemoveAll()
	{
		foreach (KeyValuePair<string, AssetBundleRequest> item in mAssetDic)
		{
			item.Value.Unload();
		}
		mAssetDic.Clear();
		Debug.Log("Remove all asset bundles", DebugTraceManager.TraceType.ASSETBUNDLE);
	}

	private IEnumerator ExecuteRequest()
	{
		int count = mExecuteList.Count;
		for (int i = 0; i < count; i++)
		{
			AssetBundleRequest req = mExecuteList[i];
			bool cancel = false;
			bool exec = true;
			if (req.isCancel)
			{
				exec = false;
			}
			else
			{
				req.Load();
			}
			while (exec)
			{
				float spendTime = 0f;
				while (!req.IsLoaded())
				{
					float waitTime = req.UpdateElapsedTime(spendTime);
					if (req.IsTimeOut())
					{
						Debug.Log("!!!!!!!!! AssetBundle TimeOut !!!!!!!!!", DebugTraceManager.TraceType.ASSETBUNDLE);
						break;
					}
					float startTime = Time.realtimeSinceStartup;
					do
					{
						yield return null;
						spendTime = Time.realtimeSinceStartup - startTime;
					}
					while (spendTime < waitTime);
				}
				if (mCancelState == CancelState.CANCELING)
				{
					Debug.LogWarning("-------------- AssetBundleRequest.ExecuteRequest Cancel --------------", DebugTraceManager.TraceType.ASSETBUNDLE);
					mCancelState = CancelState.CANCELED;
					mExecuting = false;
					exec = false;
					cancel = true;
					continue;
				}
				req.Result();
				if (req.IsRetry())
				{
					yield return new WaitForSeconds(2f);
					Debug.LogWarning("!!!!!! Load Retry [" + req.TryCount + "/" + req.MaxTryCount + "] : " + req.Url, DebugTraceManager.TraceType.ASSETBUNDLE);
					req.Load();
				}
				else
				{
					if (req.IsFailed())
					{
						mAssetDic.Remove(req.path);
					}
					exec = false;
					mRemainingList.Remove(req);
				}
			}
			if (cancel)
			{
				break;
			}
		}
		mExecuteList.Clear();
		mRemainingList.Clear();
		mExecuting = false;
		ClearCancel();
	}

	public bool IsAssetStandby(AssetBundleRequest request)
	{
		if (request.assetbundleResult == null)
		{
			return false;
		}
		return true;
	}

	public void RequestWaitAsset(AssetBundleRequest request)
	{
		if (IsAssetStandby(request))
		{
			AssetBundleResult assetbundleResult = request.assetbundleResult;
			if (request.returnObject != null)
			{
				MsgAssetBundleResponseSucceed value = new MsgAssetBundleResponseSucceed(request, assetbundleResult);
				request.returnObject.SendMessage("AssetBundleResponseSucceed", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			WaitInfo waitInfo = new WaitInfo();
			waitInfo.mRequest = request;
			StartCoroutine("WaitAsset", waitInfo);
		}
	}

	private IEnumerator WaitAsset(WaitInfo info)
	{
		while (!IsAssetStandby(info.mRequest))
		{
			yield return new WaitForSeconds(0.1f);
		}
		AssetBundleResult result = info.mRequest.CreateResult();
		if (info.mRequest.returnObject != null)
		{
			MsgAssetBundleResponseSucceed msg = new MsgAssetBundleResponseSucceed(info.mRequest, result);
			info.mRequest.returnObject.SendMessage("AssetBundleResponseSucceed", msg, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Cancel()
	{
		if (0 >= mExecuteList.Count)
		{
			mCancelState = CancelState.IDLE;
			return;
		}
		mCancelRequest = true;
		mCancelState = CancelState.CANCELING;
	}

	public bool IsCanceled()
	{
		if (mCancelState == CancelState.IDLE)
		{
			return true;
		}
		return CancelState.CANCELED == mCancelState;
	}

	public bool IsCancelRequest()
	{
		return mCancelRequest;
	}

	public void ClearCancel()
	{
		mCancelRequest = false;
		mCancelState = CancelState.IDLE;
	}

	public int GetRemainingCount()
	{
		if (mRemainingList == null)
		{
			return 0;
		}
		return mRemainingList.Count;
	}

	public AssetBundleRequest GetRemainingRequest(int index)
	{
		if (mRemainingList != null && 0 <= index && mRemainingList.Count > index)
		{
			return mRemainingList[index];
		}
		return null;
	}

	public void DebugPrintLoadedList()
	{
		string text = string.Empty;
		foreach (string key in mAssetDic.Keys)
		{
			text = text + key + "\n";
		}
		Debug.Log(string.Concat(this, ".DebugPrintLoadedList : \n", text), DebugTraceManager.TraceType.ASSETBUNDLE);
	}

	public void RequestUnload(string url)
	{
		mReqUnloadList.Add(url);
	}
}
