// Comment this out to disable trace button
#define SHOW_TRACE_MANAGER

using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugTraceManager : MonoBehaviour
{
	public enum TraceType
	{
		ALL = 0,
		SERVER = 1,
		ASSETBUNDLE = 2,
		UI = 3,
		GAME = 4,
		NUM = 5,
		BEGIN = 0,
		END = 4
	}

	public static readonly string[] TypeName = new string[5]
	{
		"All",
		"Server",
		"AssetBundle",
		"UI",
		"Game"
	};

	private static DebugTraceManager m_instance = null;

	private List<DebugTrace>[] m_traceList = new List<DebugTrace>[5];

	private StringBuilder[] m_textList = new StringBuilder[5];

	private DebugTraceMenu m_menu;

	public static DebugTraceManager Instance
	{
		get
		{
			return m_instance;
		}
		private set
		{
		}
	}

	private void Awake()
	{
#if SHOW_TRACE_MANAGER
        if (DebugTraceManager.m_instance == null)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            DebugTraceManager.m_instance = this;
            this.Init();
        }
        else
        {
	        Object.Destroy(base.gameObject);
        }
#else
        Object.Destroy(base.gameObject);
#endif
	}

	private void OnDestroy()
	{
	}

	public string GetTraceText(TraceType type)
	{
		return m_textList[(int)type].ToString();
	}

	public void AddTrace(TraceType type, DebugTrace trace)
	{
		if (type != 0)
		{
			m_traceList[(int)type].Add(trace);
			m_textList[(int)type].Append("+" + trace.text + "\n");
		}
		m_traceList[0].Add(trace);
		m_textList[0].Append("+" + trace.text + "\n");
	}

	public void ClearTrace(TraceType type)
	{
		if (type == TraceType.ALL)
		{
			for (int i = 0; i < 5; i++)
			{
				List<DebugTrace> list = m_traceList[i];
				if (list != null)
				{
					list.Clear();
					m_textList[i].Length = 0;
				}
			}
		}
		else
		{
			List<DebugTrace> list2 = m_traceList[(int)type];
			if (list2 != null)
			{
				list2.Clear();
			}
			m_textList[(int)type].Length = 0;
		}
	}

	public bool IsTracing()
	{
		if (m_menu != null && m_menu.currentState == DebugTraceMenu.State.ON)
		{
			return true;
		}
		return false;
	}

	private void Init()
	{
		for (int i = 0; i < 5; i++)
		{
			m_traceList[i] = new List<DebugTrace>();
			m_textList[i] = new StringBuilder();
			m_textList[i].Capacity = 1048576;
		}
		m_menu = base.gameObject.AddComponent<DebugTraceMenu>();
	}
}
