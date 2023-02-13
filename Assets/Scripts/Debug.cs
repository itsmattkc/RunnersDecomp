using System;
using UnityEngine;

public static class Debug
{
	public static void Break()
	{
		if (IsEnable())
		{
			UnityEngine.Debug.Break();
		}
	}

	public static void Log(object message)
	{
		if (!IsEnable())
		{
			return;
		}
		UnityEngine.Debug.Log(message);
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (instance != null)
		{
			string text = message as string;
			if (text != null)
			{
				DebugTrace trace = new DebugTrace(text);
				instance.AddTrace(DebugTraceManager.TraceType.ALL, trace);
			}
		}
	}

	public static void Log(object message, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.Log(message, context);
		}
	}

	public static void LogError(object message)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogError(message);
		}
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (instance != null)
		{
			string text = message as string;
			if (text != null)
			{
				DebugTrace trace = new DebugTrace(text);
				instance.AddTrace(DebugTraceManager.TraceType.ALL, trace);
			}
		}
	}

	public static void LogError(object message, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogError(message, context);
		}
	}

	public static void LogWarning(object message)
	{
		if (!IsEnable())
		{
			return;
		}
		UnityEngine.Debug.LogWarning(message);
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (instance != null)
		{
			string text = message as string;
			if (text != null)
			{
				DebugTrace trace = new DebugTrace(text);
				instance.AddTrace(DebugTraceManager.TraceType.ALL, trace);
			}
		}
	}

	public static void LogWarning(object message, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogWarning(message, context);
		}
	}

	public static void LogException(Exception exception)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogException(exception);
			DebugTraceManager instance = DebugTraceManager.Instance;
			if (instance != null)
			{
				DebugTrace trace = new DebugTrace(exception.ToString());
				instance.AddTrace(DebugTraceManager.TraceType.ALL, trace);
			}
		}
	}

	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogException(exception, context);
			DebugTraceManager instance = DebugTraceManager.Instance;
			if (instance != null)
			{
				DebugTrace trace = new DebugTrace(exception.ToString());
				instance.AddTrace(DebugTraceManager.TraceType.ALL, trace);
			}
		}
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end);
		}
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end, color);
		}
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end, color, duration);
		}
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
		}
	}

	public static void Log(object message, DebugTraceManager.TraceType type)
	{
		if (!IsEnable())
		{
			return;
		}
		UnityEngine.Debug.Log(message);
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (instance != null)
		{
			string text = message as string;
			if (text != null)
			{
				DebugTrace trace = new DebugTrace(text);
				instance.AddTrace(type, trace);
			}
		}
	}

	public static void LogError(object message, DebugTraceManager.TraceType type)
	{
		if (!IsEnable())
		{
			return;
		}
		UnityEngine.Debug.Log(message);
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (instance != null)
		{
			string text = message as string;
			if (text != null)
			{
				DebugTrace trace = new DebugTrace(text);
				instance.AddTrace(type, trace);
			}
		}
	}

	public static void LogWarning(object message, DebugTraceManager.TraceType type)
	{
		if (!IsEnable())
		{
			return;
		}
		UnityEngine.Debug.Log(message);
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (instance != null)
		{
			string text = message as string;
			if (text != null)
			{
				DebugTrace trace = new DebugTrace(text);
				instance.AddTrace(type, trace);
			}
		}
	}

	private static bool IsEnable()
	{
		return true;
	}
}
