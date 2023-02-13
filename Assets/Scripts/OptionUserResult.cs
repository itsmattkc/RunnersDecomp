using UnityEngine;

public class OptionUserResult : MonoBehaviour
{
	private window_user_date m_userData;

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
			if (m_userData != null)
			{
				m_userData.PlayOpenWindow();
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_user_date", true);
		}
	}

	private void SetUserData()
	{
		if (m_gameObject != null && m_userData == null)
		{
			m_userData = m_gameObject.GetComponent<window_user_date>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetUserData();
			if (m_userData != null)
			{
				m_userData.PlayOpenWindow();
			}
		}
		else if (m_userData != null && m_userData.IsEnd)
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
