using System.Diagnostics;
using UnityEngine;

public class Assert
{
	private const string ASSERT_GUARD = "UNITY_EDITOR";

	[Conditional("UNITY_EDITOR")]
	public static void True(bool test, string message)
	{
		if (!test)
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				throw new UnityException(message);
			}
			Debug.LogError(message);
			Debug.Break();
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void NotInvalidFloat(float f, string msg)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void NotInvalid(Vector3 v, string msg)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void NotInvalid(Quaternion q, string msg)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void NotInvalid(Transform t, string msg)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Fail(string message)
	{
	}
}
