using UnityEngine;

public class DebugTraceDisplayMenu : MonoBehaviour
{
	private bool m_isActive;

	private DebugTraceTextBox m_textBox;

	private DebugTraceScrollBar m_textBoxSizeBar;

	private DebugTraceScrollBar m_textScrollRatioBar;

	private DebugTraceButton m_nextTypeButton;

	private DebugTraceButton m_prevTypeButton;

	private DebugTraceButton m_clearButton;

	private DebugTraceManager.TraceType m_traceType;

	private static readonly float TextMaxScale = 4f;

	private static readonly float ScrollMaxScale = 200f;

	public void Setup()
	{
		m_textBox = base.gameObject.AddComponent<DebugTraceTextBox>();
		m_textBox.Setup(new Vector2(210f, 10f));
		m_textBoxSizeBar = base.gameObject.AddComponent<DebugTraceScrollBar>();
		m_textBoxSizeBar.SetUp("BoxSize", DebugScrollBarChangedCallback, new Vector2(10f, 200f));
		m_textScrollRatioBar = base.gameObject.AddComponent<DebugTraceScrollBar>();
		m_textScrollRatioBar.SetUp("ScrollRatio", DebugScrollBarChangedCallback, new Vector2(10f, 300f));
		m_nextTypeButton = base.gameObject.AddComponent<DebugTraceButton>();
		m_nextTypeButton.Setup("NextType", DebugButtonClickedCallback, new Vector2(110f, 100f), new Vector2(100f, 50f));
		m_prevTypeButton = base.gameObject.AddComponent<DebugTraceButton>();
		m_prevTypeButton.Setup("PrevType", DebugButtonClickedCallback, new Vector2(10f, 100f), new Vector2(100f, 50f));
		m_clearButton = base.gameObject.AddComponent<DebugTraceButton>();
		m_clearButton.Setup("Clear", DebugButtonClickedCallback, new Vector2(10f, 400f));
	}

	public void SetActive(bool isActive)
	{
		m_isActive = isActive;
		if (m_textBox != null)
		{
			m_textBox.SetActive(isActive);
		}
		if (m_textBoxSizeBar != null)
		{
			m_textBoxSizeBar.SetActive(isActive);
		}
		if (m_textScrollRatioBar != null)
		{
			m_textScrollRatioBar.SetActive(isActive);
		}
		if (m_nextTypeButton != null)
		{
			m_nextTypeButton.SetActive(isActive);
		}
		if (m_prevTypeButton != null)
		{
			m_prevTypeButton.SetActive(isActive);
		}
		if (m_clearButton != null)
		{
			m_clearButton.SetActive(isActive);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		DebugTraceManager instance = DebugTraceManager.Instance;
		if (!(instance == null) && m_textBox != null)
		{
			string text = instance.GetTraceText(m_traceType);
			if (string.IsNullOrEmpty(text))
			{
				text = "+Empty";
			}
			m_textBox.SetText(text);
		}
	}

	private void OnGUI()
	{
		if (m_isActive)
		{
			GUI.Label(new Rect(10f, 150f, 200f, 50f), DebugTraceManager.TypeName[(int)m_traceType]);
		}
	}

	private void DebugScrollBarChangedCallback(string name, float value)
	{
		if (!(m_textBox == null))
		{
			if (name == "BoxSize")
			{
				float sizeScale = value * TextMaxScale / DebugTraceScrollBar.MaxValue + 1f;
				m_textBox.SetSizeScale(sizeScale);
			}
			else if (name == "ScrollRatio")
			{
				float num = value * ScrollMaxScale / DebugTraceScrollBar.MaxValue + 1f;
				m_textBox.SetScrollScale(new Vector2(num, num));
			}
		}
	}

	private void DebugButtonClickedCallback(string buttonName)
	{
		switch (buttonName)
		{
		case "NextType":
			if (m_traceType == DebugTraceManager.TraceType.GAME)
			{
				m_traceType = DebugTraceManager.TraceType.ALL;
			}
			else
			{
				m_traceType++;
			}
			break;
		case "PrevType":
			if (m_traceType == DebugTraceManager.TraceType.ALL)
			{
				m_traceType = DebugTraceManager.TraceType.GAME;
			}
			else
			{
				m_traceType--;
			}
			break;
		case "Clear":
		{
			DebugTraceManager instance = DebugTraceManager.Instance;
			if (instance != null)
			{
				instance.ClearTrace(m_traceType);
			}
			break;
		}
		}
	}
}
