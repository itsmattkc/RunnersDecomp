using System.Collections.Generic;
using UnityEngine;

public class TimeProfiler : MonoBehaviour
{
	private Dictionary<string, float> m_checkList;

	public static bool m_active = true;

	private static TimeProfiler instance;

	public static TimeProfiler Instance
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
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
	}

	public static void StartCountTime(string index)
	{
		if (!(Instance == null) && Instance.m_checkList != null)
		{
			if (Instance.m_checkList.ContainsKey(index))
			{
				Debug.Log("TimeProfile:" + index + " is Counting Already.");
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Instance.m_checkList.Add(index, realtimeSinceStartup);
		}
	}

	public static float EndCountTime(string index)
	{
		if (Instance == null)
		{
			return 0f;
		}
		if (Instance.m_checkList == null)
		{
			return 0f;
		}
		if (Instance.m_checkList.ContainsKey(index))
		{
			float num = Instance.m_checkList[index];
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float result = realtimeSinceStartup - num;
			Debug.Log("LS:TimeProfile:Time Count:" + index + ":" + result.ToString("F3"));
			Instance.m_checkList.Remove(index);
			return result;
		}
		Debug.Log("TimeProfile:" + index + "is Not Found.");
		return 0f;
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
		Object.Destroy(base.gameObject);
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
