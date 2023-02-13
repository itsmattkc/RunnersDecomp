using UnityEngine;

public class OptionTutorial : MonoBehaviour
{
	private window_tutorial m_turoialWindow;

	private GameObject m_gameObject;

	private ui_option_scroll m_ui_option_scroll;

	private bool m_initFlag;

	public void Setup(ui_option_scroll scroll)
	{
		base.enabled = true;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		if (m_gameObject != null)
		{
			m_initFlag = true;
			m_gameObject.SetActive(true);
			if (m_turoialWindow != null)
			{
				m_turoialWindow.PlayOpenWindow();
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_tutorial", true);
		}
	}

	private void SetTuroialWindow()
	{
		if (m_gameObject != null && m_turoialWindow == null)
		{
			m_turoialWindow = m_gameObject.GetComponent<window_tutorial>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetTuroialWindow();
			if (m_turoialWindow != null)
			{
				m_turoialWindow.PlayOpenWindow();
			}
		}
		else if (m_turoialWindow != null && m_turoialWindow.IsEnd)
		{
			if (m_ui_option_scroll != null)
			{
				m_ui_option_scroll.OnEndChildPage();
			}
			base.enabled = false;
			if (m_gameObject != null)
			{
				m_gameObject.SetActive(false);
			}
		}
	}
}
