using System;
using UnityEngine;

internal class WWWRequest
{
	private WWW m_www;

	private bool m_checkTime;

	private float m_startTime;

	private bool m_isEnd;

	private bool m_isTimeOut;

	public static readonly float DefaultConnectTime = 60f;

	private float m_connectTime = DefaultConnectTime;

	public WWWRequest(string url, bool checkTime = false)
	{
		m_www = new WWW(url);
		m_startTime = Time.realtimeSinceStartup;
		m_checkTime = checkTime;
	}

	public void SetConnectTime(float connectTime)
	{
		if (connectTime > 180f)
		{
			connectTime = 180f;
		}
		m_connectTime = connectTime;
	}

	public void Remove()
	{
		try
		{
			if (m_www == null)
			{
			}
		}
		catch (Exception ex)
		{
			Debug.Log("WWWRequest.Remove():Exception->Message = " + ex.Message + ",ToString() = " + ex.ToString());
		}
		m_www = null;
	}

	public void Cancel()
	{
		Remove();
	}

	public void Update()
	{
		if (!m_isTimeOut)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = realtimeSinceStartup - m_startTime;
			if (num >= m_connectTime)
			{
				m_isTimeOut = true;
			}
		}
		if (m_isEnd)
		{
			return;
		}
		if (m_www != null)
		{
			if (m_www.isDone)
			{
				m_isEnd = true;
			}
		}
		else
		{
			m_isEnd = true;
		}
		if (m_checkTime)
		{
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			Debug.Log("LS:Load File: " + m_www.url + " Time is " + (realtimeSinceStartup2 - m_startTime));
		}
	}

	public bool IsEnd()
	{
		return m_isEnd;
	}

	public bool IsTimeOut()
	{
		return m_isTimeOut;
	}

	public string GetError()
	{
		if (m_www != null)
		{
			return m_www.error;
		}
		return null;
	}

	public byte[] GetResult()
	{
		if (m_www != null)
		{
			return m_www.bytes;
		}
		return null;
	}

	public string GetResultString()
	{
		if (m_www != null)
		{
			return m_www.text;
		}
		return null;
	}

	public int GetResultSize()
	{
		if (m_www != null)
		{
			return m_www.size;
		}
		return 0;
	}
}
