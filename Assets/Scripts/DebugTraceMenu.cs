using UnityEngine;

public class DebugTraceMenu : MonoBehaviour
{
	public enum State
	{
		OFF,
		ON
	}

	private State m_state;

	private DebugTraceButton m_offButton;

	private DebugTraceButton m_onButton;

	private DebugTraceDisplayMenu m_displayMenu;

	public State currentState
	{
		get
		{
			return m_state;
		}
		private set
		{
		}
	}

	private void Start()
	{
		bool flag = m_state == State.ON;
		m_offButton = base.gameObject.AddComponent<DebugTraceButton>();
		m_offButton.Setup("TraceOff", DebugButtonClickedCallback, new Vector2(10f, 10f));
		m_offButton.SetActive(flag);
		m_onButton = base.gameObject.AddComponent<DebugTraceButton>();
		m_onButton.Setup("TraceOn", DebugButtonClickedCallback, new Vector2(10f, 10f));
		m_onButton.SetActive(!flag);
		m_displayMenu = base.gameObject.AddComponent<DebugTraceDisplayMenu>();
		m_displayMenu.Setup();
		m_displayMenu.SetActive(flag);
	}

	private void DebugButtonClickedCallback(string buttonName)
	{
		if (buttonName == "TraceOff")
		{
			m_onButton.SetActive(true);
			m_offButton.SetActive(false);
			m_displayMenu.SetActive(false);
			GeneralWindow.Close();
			m_state = State.OFF;
		}
		else if (buttonName == "TraceOn")
		{
			m_onButton.SetActive(false);
			m_offButton.SetActive(true);
			m_displayMenu.SetActive(true);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.buttonType = GeneralWindow.ButtonType.YesNo;
			info.caption = string.Empty;
			info.message = "トレース表示中";
			GeneralWindow.Create(info);
			m_state = State.ON;
		}
	}
}
