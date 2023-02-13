using System;
using UnityEngine;

[Serializable]
public class SpawnableParameter
{
	public const float DefaultRangeIn = 20f;

	public const float DefaultRangeOut = 30f;

	private string objectname;

	private Vector3 position;

	private Quaternion rotation;

	private uint m_id;

	public float rangein = 20f;

	public float rangeout = 30f;

	public uint ID
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public string ObjectName
	{
		get
		{
			return objectname;
		}
		set
		{
			objectname = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public float RangeIn
	{
		get
		{
			return rangein;
		}
		set
		{
			rangein = value;
		}
	}

	public float RangeOut
	{
		get
		{
			return rangeout;
		}
		set
		{
			rangeout = value;
		}
	}

	public SpawnableParameter()
	{
	}

	public SpawnableParameter(string name)
	{
		objectname = name;
	}
}
