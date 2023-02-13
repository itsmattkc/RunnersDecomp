using App.Utility;
using UnityEngine;

public class SpawnableInfo
{
	private enum Flags
	{
		DESTROY,
		NOTRANGEOUT,
		REQUESTDESTROY,
		SLEEP
	}

	private enum Attribute
	{
		ONLYONE_OBJECT
	}

	public int m_block;

	public int m_blockActivateID;

	public Vector3 m_position;

	public Quaternion m_rotation;

	public SpawnableParameter m_parameters;

	private Bitset32 m_flag;

	private Bitset32 m_attribute;

	public SpawnableObject m_object;

	public ObjectSpawnManager m_manager;

	public bool Spawned
	{
		get
		{
			return m_object != null;
		}
	}

	public bool Destroyed
	{
		get
		{
			return m_flag.Test(0);
		}
	}

	public bool NotRangeOut
	{
		get
		{
			return m_flag.Test(1);
		}
		set
		{
			m_flag.Set(1, value);
		}
	}

	public bool RequestDestroy
	{
		get
		{
			return m_flag.Test(2);
		}
		set
		{
			m_flag.Set(2, value);
		}
	}

	public bool Sleep
	{
		get
		{
			return m_flag.Test(3);
		}
		set
		{
			m_flag.Set(3, value);
		}
	}

	public bool AttributeOnlyOne
	{
		get
		{
			return m_attribute.Test(0);
		}
		set
		{
			m_attribute.Set(0, value);
		}
	}

	public ObjectSpawnManager Manager
	{
		get
		{
			return m_manager;
		}
	}

	public void SpawnedObject(SpawnableObject spawnObject)
	{
		m_object = spawnObject;
	}

	public void DestroyedObject()
	{
		if ((bool)m_object)
		{
			m_object = null;
			m_flag.Set(0, true);
		}
	}
}
