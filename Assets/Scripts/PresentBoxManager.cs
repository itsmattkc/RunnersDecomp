using System.Collections.Generic;
using UnityEngine;

public class PresentBoxManager : MonoBehaviour
{
	private static PresentBoxManager instance;

	private List<PresentItem> m_present_datas;

	public static PresentBoxManager Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		if (m_present_datas == null)
		{
			m_present_datas = new List<PresentItem>();
		}
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public PresentItem GetData(int index)
	{
		if (index < m_present_datas.Count)
		{
			return m_present_datas[index];
		}
		return null;
	}

	public int GetDataCount()
	{
		return m_present_datas.Count;
	}
}
