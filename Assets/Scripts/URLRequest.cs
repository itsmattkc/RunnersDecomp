using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class URLRequest
{
	private bool mEmulation;

	private string mURL;

	private List<URLRequestParam> mParamList;

	private Action mDelegateRequest;

	private Action<WWW> mDelegateSuccess;

	private Action<WWW, bool, bool> mDelegateFailure;

	private bool mCompleted;

	private bool mNotReachability;

	private float mElapsedTime;

	private WWW mWWW;

	private string mFormString;

	public bool Completed
	{
		get
		{
			return mCompleted;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return mElapsedTime;
		}
	}

	public List<URLRequestParam> ParamList
	{
		get
		{
			return mParamList;
		}
	}

	public string FormString
	{
		get
		{
			return mFormString;
		}
	}

	public string url
	{
		get
		{
			return mURL;
		}
		set
		{
			mURL = value;
		}
	}

	public Action beginRequest
	{
		get
		{
			return mDelegateRequest;
		}
		set
		{
			mDelegateRequest = value;
		}
	}

	public Action<WWW> success
	{
		get
		{
			return mDelegateSuccess;
		}
		set
		{
			mDelegateSuccess = value;
		}
	}

	public Action<WWW, bool, bool> failure
	{
		get
		{
			return mDelegateFailure;
		}
		set
		{
			mDelegateFailure = value;
		}
	}

	public float TimeOut
	{
		get
		{
			return NetUtil.ConnectTimeOut;
		}
		private set
		{
		}
	}

	public bool Emulation
	{
		get
		{
			return mEmulation;
		}
		set
		{
			mEmulation = value;
		}
	}

	public URLRequest()
		: this(string.Empty, null, null, null)
	{
	}

	public URLRequest(string url)
		: this(url, null, null, null)
	{
	}

	public URLRequest(string url, Action begin, Action<WWW> success, Action<WWW, bool, bool> failure)
	{
		mEmulation = URLRequestManager.Instance.Emulation;
		mURL = url;
		mParamList = new List<URLRequestParam>();
		mDelegateRequest = begin;
		mDelegateSuccess = success;
		mDelegateFailure = failure;
		mCompleted = false;
		mNotReachability = false;
		mElapsedTime = 0f;
	}

	public void AddParam(string propertyName, string value)
	{
		URLRequestParam item = new URLRequestParam(propertyName, value);
		mParamList.Add(item);
	}

	public void Add1stParam(string propertyName, string value)
	{
		URLRequestParam item = new URLRequestParam(propertyName, value);
		mParamList.Insert(0, item);
	}

	public WWWForm CreateWWWForm()
	{
		if (mParamList.Count == 0)
		{
			return null;
		}
		WWWForm wWWForm = new WWWForm();
		int count = mParamList.Count;
		for (int i = 0; i < count; i++)
		{
			URLRequestParam uRLRequestParam = mParamList[i];
			if (uRLRequestParam != null)
			{
				wWWForm.AddField(uRLRequestParam.propertyName, uRLRequestParam.value);
			}
		}
		return wWWForm;
	}

	public void DidReceiveSuccess(WWW www)
	{
		if (mDelegateSuccess != null)
		{
			mDelegateSuccess(www);
		}
	}

	public void DidReceiveFailure(WWW www)
	{
		if (mDelegateFailure != null)
		{
			mDelegateFailure(www, IsTimeOut(), IsNotReachability());
		}
	}

	public void PreBegin()
	{
		if (mDelegateRequest != null)
		{
			mDelegateRequest();
		}
	}

	public void Begin()
	{
		PreBegin();
		mElapsedTime = 0f;
		WWWForm wWWForm = CreateWWWForm();
		if (wWWForm == null)
		{
			mWWW = new WWW(mURL);
			Debug.Log("URLRequestManager.ExecuteRequest:" + UriDecode(mURL), DebugTraceManager.TraceType.SERVER);
			mFormString = null;
		}
		else
		{
			mWWW = new WWW(mURL, wWWForm);
			mFormString = Encoding.ASCII.GetString(wWWForm.data);
			Debug.Log("URLRequestManager.ExecuteRequest:" + UriDecode(mURL) + "  params:" + UriDecode(mFormString), DebugTraceManager.TraceType.SERVER);
		}
	}

	public float UpdateElapsedTime(float addElapsedTime)
	{
		mElapsedTime += addElapsedTime;
		return 0.1f;
	}

	public bool IsDone()
	{
		return mWWW.isDone;
	}

	public bool IsTimeOut()
	{
		if (NetUtil.ConnectTimeOut <= mElapsedTime)
		{
			return true;
		}
		return false;
	}

	public bool IsNotReachability()
	{
		return mNotReachability;
	}

	public void Result()
	{
		if (IsTimeOut())
		{
			Debug.Log("Request : TimeOut : " + mURL, DebugTraceManager.TraceType.SERVER);
			DidReceiveFailure(null);
			mWWW = null;
			return;
		}
		if (!mWWW.isDone)
		{
			Debug.Log("WWW doesn't begin yet.", DebugTraceManager.TraceType.SERVER);
		}
		if (null == mWWW.error)
		{
			DidReceiveSuccess(mWWW);
		}
		else
		{
			DidReceiveFailure(mWWW);
		}
	}

	private static string UriDecode(string stringToUnescape)
	{
		return Uri.UnescapeDataString(stringToUnescape.Replace("+", "%20"));
	}
}
