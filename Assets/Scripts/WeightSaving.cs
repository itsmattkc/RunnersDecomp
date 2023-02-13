using UnityEngine;

public class WeightSaving : MonoBehaviour
{
	private window_performance_setting m_performanceSetting;

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
			if (m_performanceSetting != null)
			{
				m_performanceSetting.Setup();
				m_performanceSetting.PlayOpenWindow();
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_performance_setting", true);
		}
	}

	private void SetEventSetting()
	{
		if (m_gameObject != null && m_performanceSetting == null)
		{
			m_performanceSetting = m_gameObject.GetComponent<window_performance_setting>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetEventSetting();
			if (m_performanceSetting != null)
			{
				m_performanceSetting.Setup();
				m_performanceSetting.PlayOpenWindow();
			}
		}
		else if (m_performanceSetting != null && m_performanceSetting.IsEnd)
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
