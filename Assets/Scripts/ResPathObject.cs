using App;
using UnityEngine;

public class ResPathObject
{
	public enum PlaybackType
	{
		PLAYBACK_LOOP = 0,
		PLAYBACK_ONCE = 1,
		PLAYBACK_UNKNOWN = -1
	}

	private ResPathObjectData m_pathData;

	public string Name
	{
		get
		{
			AssertNoData();
			return m_pathData.name;
		}
	}

	public PlaybackType PlayBack
	{
		get
		{
			AssertNoData();
			return (PlaybackType)m_pathData.playbackType;
		}
	}

	public float Length
	{
		get
		{
			AssertNoData();
			return m_pathData.length;
		}
	}

	public int NumKeys
	{
		get
		{
			AssertNoData();
			return m_pathData.numKeys;
		}
	}

	public uint NumVertices
	{
		get
		{
			AssertNoData();
			return m_pathData.numVertices;
		}
	}

	public Bounds GetBownds
	{
		get
		{
			Bounds result = default(Bounds);
			result.SetMinMax(m_pathData.min, m_pathData.max);
			return result;
		}
	}

	public ResPathObject(ResPathObjectData data)
	{
		m_pathData = data;
	}

	public ResPathObject(ResPathObjectData data, Vector3 offsetPosition)
	{
		m_pathData = CreateMovedPathData(data, offsetPosition);
	}

	public float GetDistance(int key)
	{
		AssertNoData();
		return m_pathData.distance[key];
	}

	public Vector3 GetPosition(int key)
	{
		AssertNoData();
		return m_pathData.position[key];
	}

	public Vector3 GetNormal(int key)
	{
		AssertNoData();
		return m_pathData.normal[key];
	}

	public Vector3 GetTangent(int key)
	{
		AssertNoData();
		return m_pathData.tangent[key];
	}

	private Vector3 GetVertex(int i)
	{
		AssertNoData();
		return m_pathData.vertices[i];
	}

	public ResPathObjectData GetObjectData()
	{
		AssertNoData();
		return m_pathData;
	}

	public float NormalizeDistance(float dist)
	{
		AssertNoData();
		ResPathObjectData pathData = m_pathData;
		float length = pathData.length;
		switch (pathData.playbackType)
		{
		case 0:
			if (dist >= 0f)
			{
				return dist % length;
			}
			return dist % length + length;
		case 1:
			if (dist > length)
			{
				return length;
			}
			if (dist < 0f)
			{
				return 0f;
			}
			return dist;
		default:
			return 0f;
		}
	}

	public int GetKeyAtDistance(float dist, ref int? cache)
	{
		float dist2 = NormalizeDistance(dist);
		return GetKeyAtDistanceCore(dist2, ref cache);
	}

	public void EvaluateResult(float dist, ref Vector3? wpos, ref Vector3? normal, ref Vector3? tangent, ref int? cache)
	{
		AssertNoData();
		ResPathObjectData pathData = m_pathData;
		int num = pathData.numKeys - 1;
		int keyAtDistanceCore = GetKeyAtDistanceCore(dist, ref cache);
		if (keyAtDistanceCore >= num)
		{
			if (wpos.HasValue)
			{
				wpos = pathData.position[num];
			}
		}
		else if (wpos.HasValue)
		{
			wpos = InterpolatePosition(dist, keyAtDistanceCore, pathData.distance, pathData.position);
		}
		if (normal.HasValue)
		{
			normal = pathData.normal[keyAtDistanceCore];
		}
		if (tangent.HasValue)
		{
			tangent = pathData.tangent[keyAtDistanceCore];
		}
	}

	public void GetClosestPosition(ref Vector3 point, float from, float to, out float dist)
	{
		AssertNoData();
		ResPathObjectData pathData = m_pathData;
		float num = NormalizeDistance(from);
		float num2 = NormalizeDistance(to);
		if (pathData.playbackType == 1)
		{
			int? cache = null;
			int keyAtDistanceCore = GetKeyAtDistanceCore(num, ref cache);
			int keyAtDistanceCore2 = GetKeyAtDistanceCore(num2, ref cache);
			if (keyAtDistanceCore >= pathData.numKeys - 1)
			{
				dist = pathData.length;
				return;
			}
			Vector3 lp = InterpolatePosition(num, keyAtDistanceCore, pathData.distance, pathData.position);
			Vector3 lp2 = InterpolatePosition(num2, keyAtDistanceCore2, pathData.distance, pathData.position);
			if (keyAtDistanceCore == keyAtDistanceCore2)
			{
				float? t = 0f;
				DistanceSqPointPath(ref point, ref lp, ref lp2, ref t);
				dist = Mathf.Lerp(num, num2, t.Value);
				return;
			}
			float? t2 = 0f;
			float num3 = DistanceSqPointPath(ref point, ref lp, ref pathData.position[keyAtDistanceCore + 1], ref t2);
			float num4 = Mathf.Lerp(num, pathData.distance[keyAtDistanceCore + 1], t2.Value);
			float num5 = DistanceSqPointPath(ref point, ref pathData.position[keyAtDistanceCore2], ref lp2, ref t2);
			if (num3 > num5)
			{
				num3 = num5;
				num4 = Mathf.Lerp(pathData.distance[keyAtDistanceCore2], num2, t2.Value);
			}
			for (int i = keyAtDistanceCore + 1; i < keyAtDistanceCore2; i++)
			{
				num5 = DistanceSqPointPath(ref point, ref pathData.position[i], ref pathData.position[i + 1], ref t2);
				if (num3 > num5)
				{
					num3 = num5;
					num4 = Mathf.Lerp(pathData.distance[i], pathData.distance[i + 1], t2.Value);
				}
			}
			dist = num4;
			return;
		}
		int num6 = pathData.numKeys - 1;
		int num7;
		int num8;
		if (Mathf.Abs(from - to) >= pathData.length)
		{
			num = 0f;
			num2 = pathData.length;
			num7 = 0;
			num8 = num6 - 1;
		}
		else
		{
			int? cache2 = null;
			num7 = GetKeyAtDistance(num, ref cache2);
			num8 = GetKeyAtDistance(num2, ref cache2);
			if (num8 < num7)
			{
				num8 += num6;
			}
		}
		Vector3 lp3 = InterpolatePosition(num, num7 % num6, pathData.distance, pathData.position);
		Vector3 lp4 = InterpolatePosition(num2, num8 % num6, pathData.distance, pathData.position);
		if (num7 == num8)
		{
			float? t3 = 0f;
			DistanceSqPointPath(ref point, ref lp3, ref lp4, ref t3);
			dist = Mathf.Lerp(num, num2, t3.Value);
			return;
		}
		float? t4 = 0f;
		float num9 = DistanceSqPointPath(ref point, ref lp3, ref pathData.position[num7 % num6 + 1], ref t4);
		float num10 = Mathf.Lerp(num, pathData.distance[num7 % num6 + 1], t4.Value);
		float num11 = DistanceSqPointPath(ref point, ref pathData.position[num8 % num6], ref lp4, ref t4);
		if (num9 > num11)
		{
			num9 = num11;
			num10 = Mathf.Lerp(pathData.distance[num8 % num6], num2, t4.Value);
		}
		for (int j = num7 + 1; j < num8; j++)
		{
			int num12 = j % num6;
			num11 = DistanceSqPointPath(ref point, ref pathData.position[num12], ref pathData.position[num12 + 1], ref t4);
			if (num9 > num11)
			{
				num9 = num11;
				num10 = Mathf.Lerp(pathData.distance[num12], pathData.distance[num12 + 1], t4.Value);
			}
		}
		dist = num10;
	}

	private int GetKeyAtDistanceCore(float dist, ref int? cache)
	{
		AssertNoData();
		ResPathObjectData pathData = m_pathData;
		float[] distance = pathData.distance;
		if (cache.HasValue)
		{
			int value = cache.Value;
			if (value == pathData.numKeys - 1)
			{
				if (dist >= distance[value])
				{
					return value;
				}
			}
			else
			{
				float num = distance[value];
				float num2 = distance[value + 1];
				if (dist >= num && dist < num2)
				{
					return value;
				}
			}
		}
		int num3 = 0;
		int num4 = pathData.numKeys - 1;
		while (num4 - num3 > 1)
		{
			int num5 = (num3 + num4) / 2;
			if (dist >= distance[num5])
			{
				num3 = num5;
			}
			else
			{
				num4 = num5;
			}
		}
		int num6 = num3;
		if (cache.HasValue)
		{
			cache = num6;
		}
		return num6;
	}

	private static Vector3 InterpolatePosition(float dist, int key, float[] distanceArray, Vector3[] positionArray)
	{
		if (dist == distanceArray[key])
		{
			return positionArray[key];
		}
		float x = distanceArray[key + 1] - distanceArray[key];
		float t = (dist - distanceArray[key]) * Math.Reciprocal(x);
		return Vector3.Lerp(positionArray[key], positionArray[key + 1], t);
	}

	private static float DistanceSqPointPath(ref Vector3 point, ref Vector3 lp0, ref Vector3 lp1, ref float? t)
	{
		if (Vector3.SqrMagnitude(lp0 - lp1) < 0.01f)
		{
			t = 0f;
			return Vector3.SqrMagnitude(point - lp0);
		}
		return Segment3.DistanceSq(ref lp0, ref lp1, ref point, ref t);
	}

	private void AssertNoData()
	{
	}

	private ResPathObjectData CreateMovedPathData(ResPathObjectData src, Vector3 offset)
	{
		ResPathObjectData resPathObjectData = new ResPathObjectData();
		resPathObjectData.name = src.name;
		resPathObjectData.playbackType = src.playbackType;
		resPathObjectData.flags = src.flags;
		resPathObjectData.numKeys = src.numKeys;
		resPathObjectData.length = src.length;
		resPathObjectData.distance = new float[resPathObjectData.numKeys];
		src.distance.CopyTo(resPathObjectData.distance, 0);
		resPathObjectData.position = new Vector3[resPathObjectData.numKeys];
		for (int i = 0; i < resPathObjectData.position.Length; i++)
		{
			resPathObjectData.position[i] = src.position[i] + offset;
		}
		resPathObjectData.normal = new Vector3[resPathObjectData.numKeys];
		src.normal.CopyTo(resPathObjectData.normal, 0);
		resPathObjectData.tangent = new Vector3[resPathObjectData.numKeys];
		src.tangent.CopyTo(resPathObjectData.tangent, 0);
		resPathObjectData.numVertices = src.numVertices;
		resPathObjectData.vertices = new Vector3[resPathObjectData.numVertices];
		for (int j = 0; j < resPathObjectData.vertices.Length; j++)
		{
			resPathObjectData.vertices[j] += src.vertices[j] + offset;
		}
		resPathObjectData.min = src.min + offset;
		resPathObjectData.max = src.max + offset;
		resPathObjectData.uid = src.uid;
		return resPathObjectData;
	}
}
