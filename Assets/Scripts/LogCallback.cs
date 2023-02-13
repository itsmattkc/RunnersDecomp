using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LogCallback : MonoBehaviour
{
	public class LogData
	{
		public string m_condition;

		public string m_stackTrace;

		public string m_type;

		public LogData(string cond, string stack, string type)
		{
			m_condition = cond;
			m_stackTrace = stack;
			m_type = type;
		}
	}

	private List<LogData> m_logData = new List<LogData>();

	private Vector2 m_scrollViewVector = Vector2.zero;

	private string m_innerText;

	public bool m_saveLogFile = true;

	public bool m_offOnEditor = true;

	private static LogCallback instance;

	public static LogCallback Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, 0.1f);
	}

	private void OnEnable()
	{
		if (m_saveLogFile)
		{
			Application.RegisterLogCallback(CallbackSaveLog);
		}
		else
		{
			Application.RegisterLogCallback(HandleLog);
		}
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void HandleLog(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Log && !condition.StartsWith("LS:"))
		{
			return;
		}
		LogData item = new LogData(condition, stackTrace, type.ToString());
		m_logData.Add(item);
		if (m_logData.Count > 10)
		{
			m_logData.Remove(m_logData[0]);
		}
		m_innerText = null;
		foreach (LogData logDatum in m_logData)
		{
			string innerText = m_innerText;
			m_innerText = innerText + "condition : " + logDatum.m_condition + "\nstackTrace : " + logDatum.m_stackTrace + "\ntype : " + logDatum.m_type + "\n\n";
		}
	}

	private void CallbackSaveLog(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Log && !condition.StartsWith("LS:"))
		{
			return;
		}
		string value = string.Concat("condition : ", condition, "\nstackTrace : ", stackTrace, "\ntype : ", type, "\n\n");
		using (Stream stream = StreamOpen(true))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
			{
				try
				{
					streamWriter.Write(value);
					streamWriter.Close();
				}
				catch (Exception ex)
				{
					Debug.Log("Callback SaveLog Error:" + ex.Message);
				}
			}
		}
	}

	private void OnGUI()
	{
		if (m_innerText != null)
		{
			m_scrollViewVector = GUI.BeginScrollView(new Rect(600f, 80f, 400f, 400f), m_scrollViewVector, new Rect(0f, 0f, 350f, 10000f));
			m_innerText = GUI.TextArea(new Rect(0f, 0f, 350f, 10000f), m_innerText);
			GUI.EndScrollView();
		}
	}

	private Stream StreamOpen(bool append)
	{
		FileMode mode = (!append) ? FileMode.Create : FileMode.Append;
		return File.Open(Application.persistentDataPath + "/ErrorLog.log", mode);
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
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
}
