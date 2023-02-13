using UnityEngine;

public class ResPathObjectData
{
	public string name
	{
		get;
		set;
	}

	public byte playbackType
	{
		get;
		set;
	}

	public byte flags
	{
		get;
		set;
	}

	public ushort numKeys
	{
		get;
		set;
	}

	public float length
	{
		get;
		set;
	}

	public ushort[] knotType
	{
		get;
		set;
	}

	public float[] distance
	{
		get;
		set;
	}

	public Vector3[] position
	{
		get;
		set;
	}

	public Vector3[] normal
	{
		get;
		set;
	}

	public Vector3[] tangent
	{
		get;
		set;
	}

	public uint numVertices
	{
		get;
		set;
	}

	public Vector3[] vertices
	{
		get;
		set;
	}

	public Vector3 min
	{
		get;
		set;
	}

	public Vector3 max
	{
		get;
		set;
	}

	public ulong uid
	{
		get;
		set;
	}
}
