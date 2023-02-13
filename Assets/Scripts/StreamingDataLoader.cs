using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StreamingDataLoader : MonoBehaviour
{
	private class Info
	{
		public string url;

		public string path;

		public bool downloaded;
	}

	private enum State
	{
		Init,
		WaitLoadServerKey,
		Idle,
		LoadReady,
		Loading,
		PrepareEnd,
		End
	}

	private const string DATALIST_NAME = "StreamingDataList.txt";

	private bool m_debugInfo;

	private bool m_checkTime = true;

	private List<StreamingKeyData> m_localKeyDataList = new List<StreamingKeyData>();

	private List<StreamingKeyData> m_serverKeyDataList = new List<StreamingKeyData>();

	private bool m_error;

	private State m_state;

	private List<Info> m_downloadList = new List<Info>();

	private int m_loadEndCount;

	private int m_installFailedCount;

	private GameObject m_returnObject;

	private static StreamingDataLoader instance;

	public bool Loaded
	{
		get
		{
			return m_state == State.End;
		}
	}

	public int NumInLoadList
	{
		get
		{
			return m_downloadList.Count;
		}
	}

	public int NumLoaded
	{
		get
		{
			return m_loadEndCount;
		}
	}

	public static StreamingDataLoader Instance
	{
		get
		{
			return instance;
		}
	}

	public void Initialize(GameObject returnObject)
	{
		m_localKeyDataList.Clear();
		m_serverKeyDataList.Clear();
		LoadKeyData(SoundManager.GetDownloadedDataPath() + "StreamingDataList.txt", ref m_localKeyDataList);
		LoadServerKey(returnObject);
		m_downloadList.Clear();
		m_loadEndCount = 0;
	}

	public void LoadServerKey(GameObject returnObject)
	{
		m_returnObject = returnObject;
		m_state = State.Init;
	}

	public bool IsEnableDownlad()
	{
		if (m_state == State.Init)
		{
			return false;
		}
		if (m_state == State.WaitLoadServerKey)
		{
			return false;
		}
		return true;
	}

	public void AddFileIfNotDownloaded(string url, string path)
	{
		string fileName = Path.GetFileName(path);
		string key = GetKey(m_serverKeyDataList, fileName);
		if (IsNeedToLoad(path, fileName, key))
		{
			Info info = new Info();
			info.url = url;
			info.path = path;
			info.downloaded = false;
			m_downloadList.Add(info);
		}
	}

	public void StartDownload(int tryCount, GameObject returnObject)
	{
		m_installFailedCount = tryCount;
		if (IsEnableDownlad())
		{
			DeleteLocalGarbage();
			m_returnObject = returnObject;
			m_state = State.LoadReady;
			m_loadEndCount = 0;
		}
	}

	public void GetLoadList(ref List<string> getData)
	{
		string text = "GetLoadList \n";
		foreach (StreamingKeyData serverKeyData in m_serverKeyDataList)
		{
			getData.Add(serverKeyData.m_name);
			text = text + serverKeyData.m_name + "\n";
		}
		DebugDraw(text);
	}

	private IEnumerator DownloadDatas(GameObject returnObject)
	{
		DebugDrawList("local server", "DownloadDatas START");
		yield return null;
		bool saveFlag = false;
		while (m_loadEndCount < m_downloadList.Count)
		{
			Info info = m_downloadList[m_loadEndCount];
			if (info == null)
			{
				continue;
			}
			string fileName = Path.GetFileName(info.path);
			string serverKey = GetKey(m_serverKeyDataList, fileName);
			if (!IsNeedToLoad(info.path, fileName, serverKey))
			{
				DebugDraw("No Load");
				m_loadEndCount++;
				continue;
			}
			DebugDraw("Load !!");
			m_error = false;
			yield return StartCoroutine(UserInstallFile(info.url, info.path));
			info.downloaded = !m_error;
			if (m_error)
			{
				if (returnObject != null)
				{
					returnObject.SendMessage("StreamingDataLoad_Failed", SendMessageOptions.DontRequireReceiver);
				}
				GC.Collect();
				break;
			}
			DebugDraw("Load End");
			SetKey(ref m_localKeyDataList, fileName, serverKey);
			GC.Collect();
			m_loadEndCount++;
		}
		SaveKeyData(SoundManager.GetDownloadedDataPath() + "StreamingDataList.txt", m_localKeyDataList);
		if (!m_error)
		{
			DebugDrawList("local server", "DownloadDatas END");
			m_downloadList.Clear();
			m_state = State.PrepareEnd;
		}
	}

	private IEnumerator UserInstallFile(string url, string path)
	{
		if (m_checkTime)
		{
			Debug.Log("LS:start install URL: " + url + " path:" + path);
		}
		WWWRequest request = new WWWRequest(url);
		request.SetConnectTime(WWWRequest.DefaultConnectTime + WWWRequest.DefaultConnectTime * (float)m_installFailedCount);
		while (!request.IsEnd())
		{
			request.Update();
			if (request.IsTimeOut())
			{
				request.Cancel();
				break;
			}
			float startTime = Time.realtimeSinceStartup;
			float spendTime2 = 0f;
			do
			{
				yield return null;
				spendTime2 = Time.realtimeSinceStartup - startTime;
			}
			while (spendTime2 <= 0.1f);
		}
		Debug.Log("UserInstallFile End. ");
		if (request.IsTimeOut())
		{
			m_error = true;
			Debug.LogError("UserInstallFile TimeOut. ");
		}
		else if (request.GetError() != null)
		{
			m_error = true;
			Debug.LogError("UserInstallFile Error. " + request.GetError());
		}
		else
		{
			byte[] rowdata = request.GetResult();
			if (rowdata != null)
			{
				using (Stream stream = File.Open(path, FileMode.Create))
				{
					try
					{
						stream.Write(rowdata, 0, request.GetResultSize());
					}
					catch (Exception ex2)
					{
						Exception ex = ex2;
						m_error = true;
						Debug.Log("UserInstallFile Write Error:" + ex.Message);
					}
				}
			}
		}
		request.Remove();
	}

	private void AddKeyData(string text, ref List<StreamingKeyData> outData)
	{
		string[] array = text.Split('\n');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			string[] array3 = text2.Split(',');
			if (array3.Length == 2)
			{
				string name = array3[0].Trim();
				string key = array3[1].Trim();
				outData.Add(new StreamingKeyData(name, key));
			}
		}
	}

	private bool LoadKeyData(string filePath, ref List<StreamingKeyData> outData)
	{
		//Discarded unreachable code: IL_005b
		DebugDraw("LoadKeyData filePath=" + filePath);
		if (File.Exists(filePath))
		{
			Stream stream = null;
			try
			{
				stream = File.Open(filePath, FileMode.Open);
				using (StreamReader streamReader = new StreamReader(stream))
				{
					string text = streamReader.ReadToEnd();
					AddKeyData(text, ref outData);
				}
			}
			catch
			{
				return false;
			}
			if (stream != null)
			{
				stream.Close();
				DebugDrawList("local", "LoadKeyData");
				return true;
			}
		}
		return false;
	}

	private IEnumerator LoadURLKeyData(string url, GameObject returnObject)
	{
		DebugDraw("LoadURLKeyData url=" + url);
		WWWRequest request = new WWWRequest(url);
		while (!request.IsEnd())
		{
			request.Update();
			if (request.IsTimeOut())
			{
				request.Cancel();
				break;
			}
			float startTime = Time.realtimeSinceStartup;
			float spendTime2 = 0f;
			do
			{
				yield return null;
				spendTime2 = Time.realtimeSinceStartup - startTime;
			}
			while (spendTime2 <= 0.1f);
		}
		Debug.Log("LoadURLKeyData End. ");
		if (request.IsTimeOut())
		{
			Debug.LogError("LoadURLKeyData TimeOut. ");
			if (returnObject != null)
			{
				returnObject.SendMessage("StreamingDataLoad_Failed", SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (request.GetError() != null)
		{
			Debug.LogError("LoadURLKeyData Error. " + request.GetError());
			if (returnObject != null)
			{
				returnObject.SendMessage("StreamingDataLoad_Failed", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			try
			{
				string resultText = request.GetResultString();
				if (resultText != null)
				{
					DebugDraw("Draw WWW Text.\n" + resultText);
					AddKeyData(resultText, ref m_serverKeyDataList);
					DebugDrawList("server", "LoadURLKeyData");
				}
				else
				{
					Debug.LogWarning("text load error www.text == null " + url);
				}
			}
			catch
			{
				Debug.LogWarning("error www.text.get " + url);
			}
			if (returnObject != null)
			{
				returnObject.SendMessage("StreamingDataLoad_Succeed", SendMessageOptions.DontRequireReceiver);
			}
			m_state = State.Idle;
		}
		request.Remove();
	}

	private bool SaveKeyData(string filePath, List<StreamingKeyData> inData)
	{
		//Discarded unreachable code: IL_008f
		try
		{
			using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.GetEncoding("utf-8")))
			{
				streamWriter.NewLine = "\n";
				foreach (StreamingKeyData inDatum in inData)
				{
					streamWriter.WriteLine(inDatum.m_name + "," + inDatum.m_key);
				}
				streamWriter.Close();
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	private bool IsNeedToLoad(string path, string fileName, string serverKey)
	{
		if (!File.Exists(path))
		{
			return true;
		}
		DebugDraw("IsNeedToLoad fileName=" + fileName + " serverKey=" + serverKey);
		if (serverKey == string.Empty)
		{
			Debug.LogWarning("error : NOT serverKey");
			return false;
		}
		string key = GetKey(m_localKeyDataList, fileName);
		DebugDraw("IsNeedToLoad localKey=" + key);
		if (key == serverKey)
		{
			return false;
		}
		return true;
	}

	private string GetKey(List<StreamingKeyData> keyList, string nameData)
	{
		foreach (StreamingKeyData key in keyList)
		{
			if (nameData == key.m_name)
			{
				return key.m_key;
			}
		}
		return string.Empty;
	}

	private void SetKey(ref List<StreamingKeyData> keyList, string nameData, string key)
	{
		foreach (StreamingKeyData key2 in keyList)
		{
			if (nameData == key2.m_name)
			{
				key2.m_key = key;
				return;
			}
		}
		keyList.Add(new StreamingKeyData(nameData, key));
	}

	private bool IsStreamingKeyData(List<StreamingKeyData> keyList, string filename)
	{
		foreach (StreamingKeyData key in keyList)
		{
			if (filename == key.m_name)
			{
				return true;
			}
		}
		return false;
	}

	private void DeleteLocalGarbage()
	{
		DebugDrawList("local server", "DeleteLocalGarbage START----");
		List<StreamingKeyData> list = new List<StreamingKeyData>();
		List<string> list2 = new List<string>();
		foreach (StreamingKeyData localKeyData in m_localKeyDataList)
		{
			if (!IsStreamingKeyData(m_serverKeyDataList, localKeyData.m_name))
			{
				DebugDraw("deleteFileList.Add=" + localKeyData.m_name);
				list2.Add(localKeyData.m_name);
			}
			else
			{
				list.Add(localKeyData);
			}
		}
		if (list2.Count <= 0)
		{
			return;
		}
		foreach (string item in list2)
		{
			string text = SoundManager.GetDownloadedDataPath() + item;
			if (File.Exists(text))
			{
				DebugDraw("Delete=" + text);
				File.Delete(text);
			}
		}
		m_localKeyDataList.Clear();
		m_localKeyDataList.AddRange(list);
		SaveKeyData(SoundManager.GetDownloadedDataPath() + "StreamingDataList.txt", m_localKeyDataList);
		DebugDrawList("local", "DeleteLocalGarbage END---");
	}

	private void DebugDrawList(string type, string msg)
	{
	}

	private void DebugDraw(string msg)
	{
	}

	protected void Awake()
	{
		m_checkTime = false;
		CheckInstance();
	}

	public static void Create()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("StreamingDataLoader");
			gameObject.AddComponent<StreamingDataLoader>();
		}
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (this == instance)
		{
			instance = null;
		}
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.WaitLoadServerKey:
		case State.Idle:
		case State.Loading:
			break;
		case State.Init:
			StartCoroutine(LoadURLKeyData(SoundManager.GetDownloadURL() + "StreamingDataList.txt", m_returnObject));
			m_state = State.WaitLoadServerKey;
			break;
		case State.LoadReady:
			StartCoroutine(DownloadDatas(m_returnObject));
			m_state = State.Loading;
			break;
		case State.PrepareEnd:
			if (m_returnObject != null)
			{
				m_returnObject.SendMessage("StreamingDataLoad_Succeed", SendMessageOptions.DontRequireReceiver);
			}
			m_state = State.End;
			break;
		}
	}
}
