using System.Collections.Generic;
using UnityEngine;

public class CustomGameObject : MonoBehaviour
{
	public delegate void UpdateCustom(string updateName, float timeRate);

	public delegate void Callback(string callbackName);

	private float m_gameObjTime;

	private float m_gameObjTimeRate = 1f;

	private float m_gameObjSleepTime;

	private bool m_updateStdLast;

	private bool m_updateCusOnly = true;

	private Dictionary<string, UpdateCustom> m_updateDelegateList;

	private Dictionary<string, float> m_updateDelegateListTime;

	private Dictionary<string, float> m_updateDelegateListCurrentTime;

	private Dictionary<string, Callback> m_callbackDelegateList;

	private Dictionary<string, float> m_callbackDelegateListCurrentTime;

	public float gameObjectTime
	{
		get
		{
			return m_gameObjTime;
		}
	}

	public float gameObjectTimeRate
	{
		get
		{
			return m_gameObjTimeRate;
		}
		protected set
		{
			m_gameObjTimeRate = value;
		}
	}

	public float gameObjectSleepTime
	{
		get
		{
			return m_gameObjSleepTime;
		}
		protected set
		{
			m_gameObjSleepTime = value;
		}
	}

	public bool isSleep
	{
		get
		{
			return m_gameObjSleepTime > 0f;
		}
	}

	protected bool updateStdLast
	{
		get
		{
			return m_updateStdLast;
		}
		set
		{
			m_updateStdLast = value;
		}
	}

	protected bool updateCusOnly
	{
		get
		{
			return m_updateCusOnly;
		}
		set
		{
			m_updateCusOnly = value;
		}
	}

	private void Start()
	{
		m_gameObjTime = 0f;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (m_gameObjSleepTime <= 0f)
		{
			deltaTime *= m_gameObjTimeRate;
			if (!m_updateStdLast)
			{
				UpdateStd(deltaTime, m_gameObjTimeRate);
			}
			UpdateCustoms(deltaTime, m_gameObjTimeRate);
			if (m_updateStdLast)
			{
				UpdateStd(deltaTime, m_gameObjTimeRate);
			}
			m_gameObjTime += deltaTime;
		}
		else
		{
			m_gameObjSleepTime -= deltaTime;
			if (m_gameObjSleepTime <= 0f)
			{
				m_gameObjSleepTime = 0f;
			}
		}
	}

	private void UpdateCustoms(float deltaTime, float timeRate)
	{
		if (m_updateDelegateList != null)
		{
			int num = 0;
			Dictionary<string, UpdateCustom>.KeyCollection keys = m_updateDelegateList.Keys;
			foreach (string item in keys)
			{
				if (m_updateDelegateListCurrentTime[item] <= 0f)
				{
					if (!m_updateCusOnly || num == 0)
					{
						m_updateDelegateList[item](item, timeRate);
						m_updateDelegateListCurrentTime[item] = m_updateDelegateListTime[item];
						num++;
					}
					continue;
				}
				Dictionary<string, float> updateDelegateListCurrentTime;
				Dictionary<string, float> dictionary = updateDelegateListCurrentTime = m_updateDelegateListCurrentTime;
				string key;
				string key2 = key = item;
				float num2 = updateDelegateListCurrentTime[key];
				dictionary[key2] = num2 - deltaTime;
				if (m_updateDelegateListCurrentTime[item] <= 0f)
				{
					m_updateDelegateListCurrentTime[item] = 0f;
				}
			}
		}
		if (m_callbackDelegateList == null)
		{
			return;
		}
		Dictionary<string, Callback>.KeyCollection keys2 = m_callbackDelegateList.Keys;
		List<string> list = null;
		foreach (string item2 in keys2)
		{
			if (m_callbackDelegateListCurrentTime[item2] <= 0f)
			{
				m_callbackDelegateList[item2](item2);
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(item2);
				continue;
			}
			Dictionary<string, float> callbackDelegateListCurrentTime;
			Dictionary<string, float> dictionary2 = callbackDelegateListCurrentTime = m_callbackDelegateListCurrentTime;
			string key;
			string key3 = key = item2;
			float num2 = callbackDelegateListCurrentTime[key];
			dictionary2[key3] = num2 - deltaTime;
			if (m_callbackDelegateListCurrentTime[item2] <= 0f)
			{
				m_callbackDelegateListCurrentTime[item2] = 0f;
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (string item3 in list)
		{
			RemoveCallback(item3);
		}
	}

	protected virtual void UpdateStd(float deltaTime, float timeRate)
	{
	}

	protected bool AddUpdateCustom(UpdateCustom upd, string updName, float updTime)
	{
		bool result = false;
		if (!IsUpdateCustom(updName))
		{
			if (m_updateDelegateList == null)
			{
				m_updateDelegateList = new Dictionary<string, UpdateCustom>();
			}
			if (m_updateDelegateListTime == null)
			{
				m_updateDelegateListTime = new Dictionary<string, float>();
			}
			if (m_updateDelegateListCurrentTime == null)
			{
				m_updateDelegateListCurrentTime = new Dictionary<string, float>();
			}
			m_updateDelegateList.Add(updName, upd);
			m_updateDelegateListTime.Add(updName, updTime);
			m_updateDelegateListCurrentTime.Add(updName, updTime);
			result = true;
		}
		return result;
	}

	protected bool SetUpdateCustom(string updName, float updTime)
	{
		bool result = false;
		if (IsUpdateCustom(updName))
		{
			m_updateDelegateListTime[updName] = updTime;
			m_updateDelegateListCurrentTime[updName] = updTime;
			result = true;
		}
		return result;
	}

	protected bool RemoveUpdateCustom(string updName = null)
	{
		bool result = false;
		if (string.IsNullOrEmpty(updName))
		{
			if (m_updateDelegateList != null)
			{
				m_updateDelegateList.Clear();
			}
			if (m_updateDelegateListTime != null)
			{
				m_updateDelegateListTime.Clear();
			}
			if (m_updateDelegateListCurrentTime != null)
			{
				m_updateDelegateListCurrentTime.Clear();
			}
			m_updateDelegateList = null;
			m_updateDelegateListTime = null;
			m_updateDelegateListCurrentTime = null;
			result = true;
		}
		else if (IsUpdateCustom(updName))
		{
			m_updateDelegateList.Remove(updName);
			m_updateDelegateListTime.Remove(updName);
			m_updateDelegateListCurrentTime.Remove(updName);
			if (m_updateDelegateList.Count <= 0)
			{
				m_updateDelegateList = null;
				m_updateDelegateListTime = null;
				m_updateDelegateListCurrentTime = null;
			}
			result = true;
		}
		return result;
	}

	protected bool IsUpdateCustom(string updName)
	{
		bool result = false;
		if (m_updateDelegateList != null)
		{
			result = m_updateDelegateList.ContainsKey(updName);
		}
		return result;
	}

	protected bool AddCallback(Callback call, string callName, float callTime)
	{
		bool result = false;
		if (!IsCallback(callName))
		{
			if (m_callbackDelegateList == null)
			{
				m_callbackDelegateList = new Dictionary<string, Callback>();
			}
			if (m_callbackDelegateListCurrentTime == null)
			{
				m_callbackDelegateListCurrentTime = new Dictionary<string, float>();
			}
			m_callbackDelegateList.Add(callName, call);
			m_callbackDelegateListCurrentTime.Add(callName, callTime);
			result = true;
		}
		return result;
	}

	protected bool RemoveCallback(string callName = null)
	{
		bool result = false;
		if (string.IsNullOrEmpty(callName))
		{
			if (m_callbackDelegateList != null)
			{
				m_callbackDelegateList.Clear();
			}
			if (m_callbackDelegateListCurrentTime != null)
			{
				m_callbackDelegateListCurrentTime.Clear();
			}
			m_callbackDelegateList = null;
			m_callbackDelegateListCurrentTime = null;
			result = true;
		}
		else if (IsCallback(callName))
		{
			m_callbackDelegateList.Remove(callName);
			m_callbackDelegateListCurrentTime.Remove(callName);
			result = true;
		}
		return result;
	}

	protected int RemoveCallbackPartialMatch(string callName = null)
	{
		int num = 0;
		if (m_callbackDelegateList != null && m_callbackDelegateList.Count > 0)
		{
			Dictionary<string, Callback>.KeyCollection keys = m_callbackDelegateList.Keys;
			List<string> list = new List<string>();
			foreach (string item in keys)
			{
				if (item.IndexOf(callName) != -1 && IsCallback(item))
				{
					list.Add(item);
				}
			}
			{
				foreach (string item2 in list)
				{
					m_callbackDelegateList.Remove(item2);
					m_callbackDelegateListCurrentTime.Remove(item2);
					num++;
				}
				return num;
			}
		}
		return num;
	}

	protected bool IsCallback(string callName)
	{
		bool result = false;
		if (m_callbackDelegateList != null)
		{
			result = m_callbackDelegateList.ContainsKey(callName);
		}
		return result;
	}

	public void ResetGameObjTime()
	{
		m_gameObjTime = 0f;
	}
}
