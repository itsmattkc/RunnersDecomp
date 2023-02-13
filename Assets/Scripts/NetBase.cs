using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetBase
{
	public enum emState
	{
		Executing,
		Succeeded,
		AvailableFailed,
		UnavailableFailed
	}

	private static string mUndefinedComparer = "ERROR_CODE(32)";

	private string m_netBaseStr = "close";

	private string m_jMsgStr = "MessageJP";

	private string m_eMsgStr = "MessageEN";

	protected bool mEmulation;

	private URLRequest mRequest;

	private JsonData mResultJson;

	private JsonData mResultParamJson;

	private string mActionName;

	private string mJsonFromDll = string.Empty;

	private int mDebugLogDisplayLevel;

	private bool mMaintenance;

	private float mStartTime;

	private float mEndTime;

	private bool m_secureFlag = true;

	public static string undefinedComparer
	{
		set
		{
			mUndefinedComparer = value;
		}
	}

	protected JsonWriter paramWriter
	{
		get;
		private set;
	}

	public emState state
	{
		get;
		protected set;
	}

	public int result
	{
		get;
		set;
	}

	public ServerInterface.StatusCode resultStCd
	{
		get
		{
			return (ServerInterface.StatusCode)result;
		}
		set
		{
			result = (int)value;
		}
	}

	public string errorMessage
	{
		get;
		private set;
	}

	public int meintenanceValue
	{
		get;
		private set;
	}

	public int dataVersion
	{
		get;
		private set;
	}

	public string meintenanceMessage
	{
		get;
		private set;
	}

	public float elapsedTime
	{
		get;
		private set;
	}

	public int versionValue
	{
		get;
		private set;
	}

	public string versionString
	{
		get;
		private set;
	}

	public int assetVersionValue
	{
		get;
		private set;
	}

	public string assetVersionString
	{
		get;
		private set;
	}

	public string infoVersionString
	{
		get;
		private set;
	}

	public long serverTime
	{
		get;
		private set;
	}

	public ulong seqNum
	{
		get;
		private set;
	}

	public int clientDataVersion
	{
		get;
		private set;
	}

	protected bool enableUndefinedCompare
	{
		private get;
		set;
	}

	public URLRequest urlRequest
	{
		get
		{
			return mRequest;
		}
	}

	public Action<NetBase> responseEvent
	{
		private get;
		set;
	}

	public static long LastNetServerTime
	{
		get;
		set;
	}

	private static long TimeDifference
	{
		get;
		set;
	}

	public NetBase()
	{
		mEmulation = false;
		mRequest = new URLRequest();
		paramWriter = new JsonWriter();
		state = emState.Executing;
		mResultJson = null;
		mResultParamJson = null;
		mActionName = null;
		elapsedTime = 0f;
		enableUndefinedCompare = true;
		responseEvent = null;
	}

	public void Request()
	{
		Debug.Log(string.Concat("NetBase.Request [", this, "]"), DebugTraceManager.TraceType.SERVER);
		state = emState.Executing;
		mRequest.beginRequest = BeginRequest;
		mRequest.success = DidSuccess;
		mRequest.failure = DidFailure;
		mRequest.Emulation = mEmulation;
		elapsedTime = 0f;
		mStartTime = Time.realtimeSinceStartup;
		URLRequestManager.Instance.Request(mRequest);
	}

	private void BeginRequest()
	{
		paramWriter.WriteObjectStart();
		SetCommonRequestParam();
		DoRequest();
		SetRequestUrl();
		SetRequestParam();
		SetSecureRequestParam();
		paramWriter.Reset();
		paramWriter = null;
	}

	private void SetRequestUrl()
	{
		mRequest.url = NetBaseUtil.ActionServerURL + mActionName;
	}

	private bool IsJsonFromDll()
	{
		if (string.IsNullOrEmpty(mJsonFromDll))
		{
			return false;
		}
		return true;
	}

	private bool IsSecure()
	{
		if (!IsJsonFromDll())
		{
			return false;
		}
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance == null)
		{
			return false;
		}
		if (!instance.IsSecure())
		{
			return false;
		}
		return true;
	}

	protected void SetSecureFlag(bool flag)
	{
		m_secureFlag = flag;
	}

	private void SetCommonRequestParam()
	{
		ServerLoginState loginState = ServerInterface.LoginState;
		bool isLoggedIn = loginState.IsLoggedIn;
		string sessionId = loginState.sessionId;
		string version = CurrentBundleVersion.version;
		ulong num = loginState.seqNum + 1;
		if (isLoggedIn)
		{
			WriteActionParamValue("sessionId", sessionId);
		}
		WriteActionParamValue("version", version);
		WriteActionParamValue("seq", num);
	}

	private void SetRequestParam()
	{
		paramWriter.WriteObjectEnd();
		string empty = string.Empty;
		empty = ((!IsJsonFromDll()) ? paramWriter.ToString() : mJsonFromDll);
		if (0 < empty.Length)
		{
			mRequest.AddParam("param", empty);
		}
	}

	private void SetSecureRequestParam()
	{
		string value = "0";
		string value2 = string.Empty;
		if (IsSecure())
		{
			value = "1";
			value2 = CryptoUtility.code;
		}
		mRequest.AddParam("secure", value);
		mRequest.AddParam("key", value2);
	}

	public void DidSuccess(WWW www)
	{
		if (www != null)
		{
			DidSuccess(www.text);
		}
		else
		{
			DidSuccess(string.Empty);
		}
	}

	public void DidSuccess(string responseText)
	{
		mEndTime = Time.realtimeSinceStartup;
		elapsedTime = mEndTime - mStartTime;
		OutputResponseText(responseText);
		state = emState.Succeeded;
		if (URLRequestManager.Instance.Emulation || mEmulation)
		{
			DoEmulation();
		}
		else
		{
			if (responseText.Length == 0)
			{
				Debug.Log(string.Concat(this, " responce is empty."), DebugTraceManager.TraceType.SERVER);
				state = emState.UnavailableFailed;
				return;
			}
			if (IsUndefinedAction(responseText))
			{
				DoEmulation();
			}
			else
			{
				mResultJson = JsonMapper.ToObject(responseText);
				bool flag = false;
				if (IsJsonFromDll())
				{
					string text = CryptoUtility.code = NetUtil.GetJsonString(mResultJson, "key");
					mResultParamJson = mResultJson;
					string jsonString2 = NetUtil.GetJsonString(mResultJson, "param");
					CPlusPlusLink instance = CPlusPlusLink.Instance;
					if (instance != null && jsonString2 != null)
					{
						Debug.Log("CPlusPlusLink.DecodeServerResponseText");
						responseText = instance.DecodeServerResponseText(jsonString2);
						mResultParamJson = JsonMapper.ToObject(responseText);
						Debug.Log("DecryptString = " + responseText);
						CryptoUtility.code = jsonString2.Substring(0, 16);
						flag = true;
					}
				}
				if (!flag)
				{
					mResultParamJson = NetUtil.GetJsonObject(mResultJson, "param");
				}
				AnalyazeCommonParam(false);
				if (IsMaintenance())
				{
					return;
				}
				switch (resultStCd)
				{
				case ServerInterface.StatusCode.Ok:
					break;
				case ServerInterface.StatusCode.VersionForApplication:
				case ServerInterface.StatusCode.ReceiveFailureMessage:
				case ServerInterface.StatusCode.AlreadySentEnergy:
				case ServerInterface.StatusCode.AlreadyRequestedEnergy:
				case ServerInterface.StatusCode.AlreadyInvitedFriend:
				case ServerInterface.StatusCode.RouletteUseLimit:
				case ServerInterface.StatusCode.EnergyLimitPurchaseTrigger:
				case ServerInterface.StatusCode.CharacterLevelLimit:
				case ServerInterface.StatusCode.NotEnoughEnergy:
				case ServerInterface.StatusCode.NotEnoughRings:
				case ServerInterface.StatusCode.NotEnoughRedStarRings:
				case ServerInterface.StatusCode.AlreadyRemovedPrePurchase:
				case ServerInterface.StatusCode.AlreadyExistedPrePurchase:
				case ServerInterface.StatusCode.UsedSerialCode:
				case ServerInterface.StatusCode.InvalidSerialCode:
				case ServerInterface.StatusCode.PassWordError:
					state = emState.AvailableFailed;
					if (IsEscapeErrorMode())
					{
						DoEscapeErrorMode(mResultParamJson);
					}
					return;
				case ServerInterface.StatusCode.TimeOut:
					state = emState.UnavailableFailed;
					result = -7;
					return;
				default:
					state = emState.UnavailableFailed;
					return;
				}
				if (mResultParamJson != null)
				{
					DoDidSuccess(mResultParamJson);
				}
			}
		}
		if (responseEvent != null)
		{
			responseEvent(this);
		}
	}

	public void DidFailure(WWW www, bool timeOut, bool notReachability)
	{
		if (notReachability)
		{
			Debug.LogWarning(string.Concat("!!!!!!!!!!!!!!!!!!!!!!!!!!! ", this, ".DidFailure : NotReachability"), DebugTraceManager.TraceType.SERVER);
			resultStCd = ServerInterface.StatusCode.NotReachability;
		}
		else if (timeOut)
		{
			Debug.LogWarning(string.Concat("!!!!!!!!!!!!!!!!!!!!!!!!!!! ", this, ".DidFailure : TimeOut"), DebugTraceManager.TraceType.SERVER);
			resultStCd = ServerInterface.StatusCode.TimeOut;
		}
		else if (www.error != null)
		{
			Debug.LogWarning(string.Concat("!!!!!!!!!!!!!!!!!!!!!!!!!!! ", this, ".DidFailure : ", www.error), DebugTraceManager.TraceType.SERVER);
			bool flag = www.error.Contains("400") || www.error.Contains("401") || www.error.Contains("402") || www.error.Contains("403") || www.error.Contains("404") || www.error.Contains("405") || www.error.Contains("406") || www.error.Contains("407") || www.error.Contains("408") || www.error.Contains("409") || www.error.Contains("410") || www.error.Contains("411") || www.error.Contains("412") || www.error.Contains("413") || www.error.Contains("414") || www.error.Contains("415");
			bool flag2 = www.error.Contains("502") || www.error.Contains("503") || www.error.Contains("504");
			bool flag3 = www.error.Contains("500") || www.error.Contains("501") || www.error.Contains("505");
			if (flag)
			{
				resultStCd = ServerInterface.StatusCode.CliendError;
			}
			else if (flag3)
			{
				resultStCd = ServerInterface.StatusCode.InternalServerError;
			}
			else if (flag2)
			{
				resultStCd = ServerInterface.StatusCode.ServerBusy;
			}
			else if (www.error.Contains("unreachable"))
			{
				resultStCd = ServerInterface.StatusCode.NotReachability;
			}
			else
			{
				resultStCd = ServerInterface.StatusCode.NotReachability;
			}
		}
		else
		{
			resultStCd = ServerInterface.StatusCode.OtherError;
		}
		state = emState.UnavailableFailed;
	}

	private void DoEmulation()
	{
		Debug.LogWarning(string.Concat(this, ".DidSuccess : Emulation"), DebugTraceManager.TraceType.SERVER);
		AnalyazeCommonParam(true);
		if (resultStCd == ServerInterface.StatusCode.Ok)
		{
			DoDidSuccessEmulation();
		}
	}

	private bool IsUndefinedAction(string result)
	{
		if (!enableUndefinedCompare)
		{
			return false;
		}
		if (!result.Contains(mUndefinedComparer))
		{
			return false;
		}
		return true;
	}

	private void OutputResponseText(string text)
	{
		if (text != null)
		{
		}
	}

	private void AnalyazeCommonParam(bool emulation)
	{
		result = 0;
		errorMessage = string.Empty;
		meintenanceValue = 0;
		dataVersion = -1;
		meintenanceMessage = string.Empty;
		versionString = "1.0.0";
		infoVersionString = string.Empty;
		versionValue = NetBaseUtil.GetVersionValue(versionString);
		assetVersionString = "1.0.0";
		assetVersionValue = NetBaseUtil.GetVersionValue(assetVersionString);
		serverTime = 0L;
		seqNum = 0uL;
		ServerInterface.LoginState.serverVersionValue = versionValue;
		if (!emulation)
		{
			result = NetUtil.GetJsonInt(mResultParamJson, "statusCode");
			errorMessage = NetUtil.GetJsonString(mResultParamJson, "errorMessage");
			meintenanceValue = NetUtil.GetJsonInt(mResultParamJson, "maintenance");
			dataVersion = NetUtil.GetJsonInt(mResultParamJson, "data_version");
			meintenanceMessage = NetUtil.GetJsonString(mResultParamJson, "maintenance_text");
			versionString = NetUtil.GetJsonString(mResultParamJson, "version");
			versionValue = NetBaseUtil.GetVersionValue(versionString);
			assetVersionString = NetUtil.GetJsonString(mResultParamJson, "assets_version");
			assetVersionValue = NetBaseUtil.GetVersionValue(assetVersionString);
			infoVersionString = NetUtil.GetJsonString(mResultParamJson, "info_version");
			serverTime = NetUtil.GetJsonLong(mResultParamJson, "server_time");
			string jsonString = NetUtil.GetJsonString(mResultParamJson, m_netBaseStr + "Url");
			string jsonString2 = NetUtil.GetJsonString(mResultParamJson, m_netBaseStr + m_eMsgStr);
			string jsonString3 = NetUtil.GetJsonString(mResultParamJson, m_netBaseStr + m_jMsgStr);
			seqNum = (ulong)NetUtil.GetJsonLong(mResultParamJson, "seq");
			clientDataVersion = NetUtil.GetJsonInt(mResultParamJson, "client_data_version");
			LastNetServerTime = serverTime;
			TimeDifference = serverTime - NetUtil.GetCurrentUnixTime();
			ServerInterface.LoginState.seqNum = seqNum;
			ServerInterface.LoginState.DataVersion = dataVersion;
			ServerInterface.LoginState.AssetsVersion = assetVersionValue;
			ServerInterface.LoginState.AssetsVersionString = assetVersionString;
			ServerInterface.LoginState.InfoVersionString = infoVersionString;
			ServerInterface.NextVersionState.m_buyRSRNum = NetUtil.GetJsonInt(mResultParamJson, "numBuyRedRingsANDROID");
			ServerInterface.NextVersionState.m_freeRSRNum = NetUtil.GetJsonInt(mResultParamJson, "numRedRingsANDROID");
			ServerInterface.NextVersionState.m_userName = NetUtil.GetJsonString(mResultParamJson, "userName");
			ServerInterface.NextVersionState.m_url = jsonString;
			ServerInterface.NextVersionState.m_eMsg = jsonString2;
			ServerInterface.NextVersionState.m_jMsg = jsonString3;
		}
		else
		{
			serverTime = NetUtil.GetCurrentUnixTime();
			LastNetServerTime = serverTime;
			TimeDifference = serverTime - NetUtil.GetCurrentUnixTime();
		}
		if (result != 0)
		{
			Debug.LogWarning(">>>>>>>>>>>>> " + ToString() + " : Result = " + result + " <<<<<<<<<<<<<");
		}
	}

	public bool IsExecuting()
	{
		if (state == emState.Executing)
		{
			return true;
		}
		return false;
	}

	public bool IsSucceeded()
	{
		if (state == emState.Succeeded && !IsMaintenance())
		{
			return true;
		}
		return false;
	}

	public bool IsFailed()
	{
		if (state == emState.AvailableFailed || state == emState.UnavailableFailed)
		{
			return true;
		}
		return false;
	}

	public bool IsAvailableFailed()
	{
		if (state == emState.AvailableFailed)
		{
			return true;
		}
		return false;
	}

	public bool IsUnavailableFailed()
	{
		if (state == emState.UnavailableFailed)
		{
			return true;
		}
		return false;
	}

	public bool IsNotReachability()
	{
		if (IsFailed() && resultStCd == ServerInterface.StatusCode.NotReachability)
		{
			return true;
		}
		return false;
	}

	public bool IsNoAccessTimeOut()
	{
		if (IsFailed() && (resultStCd == ServerInterface.StatusCode.ExpirationSession || resultStCd == ServerInterface.StatusCode.TimeOut))
		{
			return true;
		}
		return false;
	}

	public bool IsNeededLoginFailed()
	{
		if (IsFailed())
		{
		}
		return false;
	}

	public bool IsMaintenance()
	{
		return 0 != meintenanceValue;
	}

	protected void SetAction(string action)
	{
		mActionName = action + "/?";
	}

	protected void WriteActionParamValue(string propertyName, object value)
	{
		NetUtil.WriteValue(paramWriter, propertyName, value);
	}

	protected void WriteActionParamObject(string objectName, Dictionary<string, object> properties)
	{
		NetUtil.WriteObject(paramWriter, objectName, properties);
	}

	protected void WriteActionParamArray(string arrayName, List<object> objects)
	{
		NetUtil.WriteArray(paramWriter, arrayName, objects);
	}

	protected void WriteJsonString(string jsonString)
	{
		mJsonFromDll = jsonString;
	}

	protected void DisplayDebugInfo()
	{
		mDebugLogDisplayLevel = 0;
		DebugLogInner(mResultParamJson);
	}

	private void DebugLogInner(JsonData jdata)
	{
		mDebugLogDisplayLevel++;
		string text = string.Empty;
		for (int i = 0; i < mDebugLogDisplayLevel; i++)
		{
			text += "  ";
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		int count = jdata.Count;
		for (int j = 0; j < count; j++)
		{
			JsonData jsonData = jdata[j];
			if (jsonData.IsArray)
			{
				empty = jdata.GetKey(j);
				Debug.Log(text + "ARRAY  key[" + empty + "]", DebugTraceManager.TraceType.SERVER);
			}
			else if (jsonData.IsInt)
			{
				empty = jdata.GetKey(j);
				empty2 = (string)jdata[j];
				Debug.Log(text + "INT    key[" + empty + "]  value[]" + empty2, DebugTraceManager.TraceType.SERVER);
			}
			else if (jsonData.IsLong)
			{
				empty = jdata.GetKey(j);
				empty2 = (string)jdata[j];
				Debug.Log(text + "LONG   key[" + empty + "]  value[]" + empty2, DebugTraceManager.TraceType.SERVER);
			}
			else if (jsonData.IsObject)
			{
				Debug.Log(text + "OBJECT[" + j + "]", DebugTraceManager.TraceType.SERVER);
			}
			else if (jsonData.IsString)
			{
				empty = jdata.GetKey(j);
				empty2 = (string)jdata[j];
				Debug.Log(text + "STRING :  key[" + empty + "]  value[" + empty2 + "]", DebugTraceManager.TraceType.SERVER);
			}
		}
		mDebugLogDisplayLevel--;
	}

	protected bool GetFlag(JsonData jdata, string key)
	{
		if (NetUtil.GetJsonInt(jdata, key) != 0)
		{
			return true;
		}
		return false;
	}

	protected virtual bool IsEscapeErrorMode()
	{
		return false;
	}

	protected virtual void DoEscapeErrorMode(JsonData jdata)
	{
	}

	protected abstract void DoRequest();

	protected abstract void DoDidSuccess(JsonData jdata);

	protected abstract void DoDidSuccessEmulation();

	public static DateTime GetCurrentTime()
	{
		return NetUtil.GetLocalDateTime(NetUtil.GetCurrentUnixTime() + TimeDifference);
	}
}
