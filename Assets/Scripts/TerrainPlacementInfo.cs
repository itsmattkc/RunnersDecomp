using UnityEngine;

public class TerrainPlacementInfo
{
	private int m_reserveIndex = -1;

	private bool m_destroyed;

	public int m_terrainIndex
	{
		get;
		set;
	}

	public TerrainBlock m_block
	{
		get;
		set;
	}

	public GameObject m_gameObject
	{
		get;
		set;
	}

	public int ReserveIndex
	{
		get
		{
			return m_reserveIndex;
		}
		set
		{
			m_reserveIndex = value;
		}
	}

	public bool Created
	{
		get
		{
			return m_gameObject != null;
		}
	}

	public bool Destroyed
	{
		get
		{
			return m_destroyed;
		}
	}

	public TerrainPlacementInfo()
	{
		m_destroyed = false;
	}

	public bool IsReserveTerrain()
	{
		return m_reserveIndex != -1;
	}

	public void DestroyObject()
	{
		m_gameObject = null;
		m_destroyed = true;
	}
}
