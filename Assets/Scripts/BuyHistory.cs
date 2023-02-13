using UnityEngine;

public class BuyHistory : MonoBehaviour
{
	private window_buying_history m_history;

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
			if (m_history != null)
			{
				m_history.PlayOpenWindow();
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_buying_history", true);
		}
	}

	private void SetBuyingHistory()
	{
		if (m_gameObject != null && m_history == null)
		{
			m_history = m_gameObject.GetComponent<window_buying_history>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetBuyingHistory();
			if (m_history != null)
			{
				m_history.PlayOpenWindow();
			}
		}
		else if (m_history != null && m_history.IsEnd)
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
