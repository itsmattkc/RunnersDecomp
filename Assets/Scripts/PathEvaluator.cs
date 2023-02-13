using App;
using System;
using UnityEngine;

public class PathEvaluator
{
	private PathComponent m_path;

	private float m_dist;

	private int m_cache;

	public float Distance
	{
		get
		{
			AssertPathNotValid();
			return m_dist;
		}
		set
		{
			AssertPathNotValid();
			m_dist = value;
		}
	}

	public PathEvaluator()
	{
	}

	public PathEvaluator(PathComponent component)
	{
		SetPathObject(component);
	}

	public PathEvaluator(PathEvaluator src)
	{
		SetPathObject(src.GetPathObject());
		Distance = src.Distance;
	}

	public void SetPathObject(PathComponent path)
	{
		m_path = path;
		m_dist = 0f;
		m_cache = 0;
	}

	public PathComponent GetPathObject()
	{
		return m_path;
	}

	public bool IsValid()
	{
		return m_path.IsValid();
	}

	private void Reset()
	{
		m_path = null;
	}

	public float GetLength()
	{
		AssertPathNotValid();
		ResPathObject resPath = GetResPath(this);
		if (resPath != null)
		{
			return resPath.Length;
		}
		return 0f;
	}

	public Vector3 GetWorldPosition(float dist)
	{
		Vector3? wpos = default(Vector3);
		Vector3? normal = null;
		GetPNT(dist, ref wpos, ref normal, ref normal);
		return wpos.Value;
	}

	public Vector3 GetWorldPosition()
	{
		return GetWorldPosition(Distance);
	}

	public Vector3 GetNormal(float dist)
	{
		Vector3? normal = default(Vector3);
		Vector3? wpos = null;
		GetPNT(dist, ref wpos, ref normal, ref wpos);
		return normal.Value;
	}

	public Vector3 GetNormal()
	{
		return GetNormal(Distance);
	}

	public Vector3 GetTangent(float dist)
	{
		Vector3? tangent = default(Vector3);
		Vector3? wpos = null;
		GetPNT(dist, ref wpos, ref wpos, ref tangent);
		return tangent.Value;
	}

	public Vector3 GetTangent()
	{
		return GetTangent(Distance);
	}

	public void GetPNT(float dist, ref Vector3? wpos, ref Vector3? normal, ref Vector3? tangent)
	{
		AssertPathNotValid();
		ResPathObject resPath = GetResPath(this);
		if (resPath != null)
		{
			int? cache = m_cache;
			resPath.EvaluateResult(dist, ref wpos, ref normal, ref tangent, ref cache);
			m_cache = cache.Value;
		}
	}

	public void GetPNT(ref Vector3? wpos, ref Vector3? normal, ref Vector3? tangent)
	{
		GetPNT(Distance, ref wpos, ref normal, ref tangent);
	}

	public void Advance(float delta)
	{
		AssertPathNotValid();
		ResPathObject resPath = GetResPath(this);
		if (resPath != null)
		{
			m_dist = resPath.NormalizeDistance(m_dist + delta);
		}
	}

	public void GetClosestPositionAlongSpline(Vector3 point, float from, float to, out float dist)
	{
		AssertPathNotValid();
		ResPathObject resPath = GetResPath(this);
		if (resPath == null)
		{
			dist = 0f;
		}
		else
		{
			resPath.GetClosestPosition(ref point, from, to, out dist);
		}
	}

	public float GetClosestPositionAlongSpline(Vector3 point, float from, float to)
	{
		float dist;
		GetClosestPositionAlongSpline(point, from, to, out dist);
		return dist;
	}

	public float GetClosestPositionAlongSplineInterpolate(Vector3 point, float from, float to, ref Vector3? center, ref float? radius)
	{
		float closestPositionAlongSpline = GetClosestPositionAlongSpline(point, from, to);
		if (center.HasValue)
		{
			center = Vector3.zero;
		}
		if (radius.HasValue)
		{
			radius = 0f;
		}
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		ResPathObject resPathObject = GetPathObject().GetResPathObject();
		if (resPathObject == null)
		{
			return 0f;
		}
		ResPathObjectData objectData = resPathObject.GetObjectData();
		int num = objectData.numKeys - 1;
		if (num <= 1)
		{
			return closestPositionAlongSpline;
		}
		float dist = resPathObject.NormalizeDistance(closestPositionAlongSpline);
		int? cache = m_cache;
		int keyAtDistance = resPathObject.GetKeyAtDistance(dist, ref cache);
		m_cache = cache.Value;
		switch (objectData.playbackType)
		{
		case 1:
		{
			int num2 = keyAtDistance;
			if (num2 == 0)
			{
				num2++;
			}
			else if (num2 >= num)
			{
				num2--;
			}
			vector2 = objectData.position[num2 - 1];
			vector = objectData.position[num2];
			vector3 = objectData.position[num2 + 1];
			break;
		}
		case 0:
			vector2 = objectData.position[(keyAtDistance != 0) ? (keyAtDistance - 1) : (num - 1)];
			vector = objectData.position[keyAtDistance];
			vector3 = objectData.position[(keyAtDistance == num) ? 1 : (keyAtDistance + 1)];
			break;
		}
		Vector3 lhs = vector2 - vector;
		Vector3 rhs = vector3 - vector;
		float magnitude = lhs.magnitude;
		float magnitude2 = rhs.magnitude;
		if (magnitude > 1E-06f && magnitude2 > 1E-06f)
		{
			float value = Vector3.Dot(lhs, rhs) / magnitude / magnitude2;
			value = Mathf.Clamp(value, -1f, 1f);
			float num3 = (float)System.Math.PI - Mathf.Acos(value);
			if (num3 > 1E-06f && (magnitude + magnitude2) * ((float)System.Math.PI * 2f / num3) < 100000f)
			{
				Vector3 dst = Vector3.Cross(lhs, rhs);
				if (!App.Math.Vector3NormalizeIfNotZero(dst, out dst))
				{
					return closestPositionAlongSpline;
				}
				Matrix4x4 m = App.Math.Matrix44OrthonormalDirection2(dst, lhs.normalized);
				Matrix4x4 matrix4x = App.Math.Matrix34InverseNonSingular(m);
				Vector3 vector4 = matrix4x.MultiplyVector(vector);
				Vector3 vector5 = matrix4x.MultiplyVector(vector2);
				Vector3 vector6 = matrix4x.MultiplyVector(vector3);
				float slant = 0f;
				float slant2 = 0f;
				float intercept = 0f;
				float intercept2 = 0f;
				bool midperpendicular2D = GetMidperpendicular2D(vector5.x, vector5.y, vector4.x, vector4.y, ref slant, ref intercept);
				bool midperpendicular2D2 = GetMidperpendicular2D(vector4.x, vector4.y, vector6.x, vector6.y, ref slant2, ref intercept2);
				if (!midperpendicular2D && !midperpendicular2D2)
				{
					return closestPositionAlongSpline;
				}
				float num4;
				float y;
				if (!midperpendicular2D)
				{
					num4 = (vector4.x + vector5.x) * 0.5f;
					y = slant2 * num4 + intercept2;
				}
				else if (!midperpendicular2D2)
				{
					num4 = (vector4.x + vector6.x) * 0.5f;
					y = slant * num4 + intercept;
				}
				else
				{
					num4 = (intercept2 - intercept) / (slant - slant2);
					y = slant * num4 + intercept;
				}
				Vector3 vector7 = default(Vector3);
				vector7.x = num4;
				vector7.y = y;
				vector7.z = vector4.z;
				float num5 = vector4.x - vector7.x;
				float num6 = vector4.y - vector7.y;
				float num7 = Mathf.Sqrt(num5 * num5 + num6 * num6);
				Vector3 a = matrix4x.MultiplyVector(point);
				Vector3 a2 = a - vector7;
				a2.z = 0f;
				float magnitude3 = a2.magnitude;
				Vector3 v = (!(magnitude3 > 1E-06f)) ? vector7 : (vector7 + a2 * (num7 / magnitude3));
				Vector3 value2 = m.MultiplyVector(vector7);
				Vector3 point2 = m.MultiplyVector(v);
				closestPositionAlongSpline = GetClosestPositionAlongSpline(point2, from, to);
				if (center.HasValue)
				{
					center = value2;
				}
				if (radius.HasValue)
				{
					radius = num7;
				}
			}
		}
		return closestPositionAlongSpline;
	}

	private static bool GetMidperpendicular2D(float sx, float sy, float ex, float ey, ref float slant, ref float intercept)
	{
		if (Mathf.Abs(sx - ex) <= 1E-06f)
		{
			slant = 0f;
			intercept = (sy + ey) * 0.5f;
			return true;
		}
		float num = (sy - ey) / (sx - ex);
		if (Mathf.Abs(num) <= 1E-06f)
		{
			return false;
		}
		float num2 = sx + (ex - sx) * 0.5f;
		float num3 = sy + (ey - sy) * 0.5f;
		slant = -1f / num;
		intercept = num3 - slant * num2;
		return true;
	}

	private void AssertPathNotValid()
	{
	}

	private static ResPathObject GetResPath(PathEvaluator self)
	{
		return self.GetPathObject().GetResPathObject();
	}
}
