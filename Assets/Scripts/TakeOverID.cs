using UnityEngine;

public class TakeOverID : MonoBehaviour
{
	private window_takeover_id m_takeOverId;

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
			if (m_takeOverId != null)
			{
				m_takeOverId.PlayOpenWindow();
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_takeover_id", true);
		}
	}

	private void SetTakeOverId()
	{
		if (m_gameObject != null && m_takeOverId == null)
		{
			m_takeOverId = m_gameObject.GetComponent<window_takeover_id>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetTakeOverId();
			if (m_takeOverId != null)
			{
				m_takeOverId.PlayOpenWindow();
			}
		}
		else if (m_takeOverId != null && m_takeOverId.IsEnd)
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
