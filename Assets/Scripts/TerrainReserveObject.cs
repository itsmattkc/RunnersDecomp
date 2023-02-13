using UnityEngine;

public class TerrainReserveObject
{
	private GameObject m_gameObject;

	private string m_blockName = string.Empty;

	private int m_reserveIndex = -1;

	private bool m_rented;

	public string blockName
	{
		get
		{
			return m_blockName;
		}
	}

	public bool EableReservation
	{
		get
		{
			return !m_rented;
		}
	}

	public int ReserveIndex
	{
		get
		{
			return m_reserveIndex;
		}
	}

	public TerrainReserveObject(GameObject obj, string name, int reserveIndex)
	{
		m_gameObject = obj;
		m_blockName = name;
		m_reserveIndex = reserveIndex;
		m_rented = false;
	}

	public GameObject ReserveObject()
	{
		m_rented = true;
		return m_gameObject;
	}

	public void ReturnObject()
	{
		m_rented = false;
		if (m_gameObject != null && m_gameObject.activeSelf)
		{
			m_gameObject.SetActive(false);
		}
	}

	public GameObject GetGameObject()
	{
		return m_gameObject;
	}
}
