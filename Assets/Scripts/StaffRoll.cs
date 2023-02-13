using UnityEngine;

public class StaffRoll : MonoBehaviour
{
	public enum TextType
	{
		STAFF_ROLL,
		COPYRIGHT
	}

	private window_staffroll m_staffRoll;

	private GameObject m_windoObj;

	private ui_option_scroll m_ui_option_scroll;

	private TextType m_textType;

	public void SetTextType(TextType type)
	{
		m_textType = type;
	}

	public void Setup(ui_option_scroll scroll)
	{
		base.enabled = true;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		if (m_windoObj == null)
		{
			m_windoObj = HudMenuUtility.GetLoadMenuChildObject("window_staffroll", false);
		}
		if (!(m_windoObj != null))
		{
			return;
		}
		if (m_staffRoll == null)
		{
			m_staffRoll = m_windoObj.GetComponent<window_staffroll>();
		}
		if (m_staffRoll != null)
		{
			switch (m_textType)
			{
			case TextType.STAFF_ROLL:
				m_staffRoll.SetStaffRollText();
				break;
			case TextType.COPYRIGHT:
				m_staffRoll.SetCopyrightText();
				break;
			}
			m_windoObj.SetActive(true);
			m_staffRoll.PlayOpenWindow();
		}
	}

	public void Update()
	{
		if (m_staffRoll != null && m_staffRoll.IsEnd)
		{
			if (m_ui_option_scroll != null)
			{
				m_ui_option_scroll.OnEndChildPage();
			}
			base.enabled = false;
			if (m_windoObj != null)
			{
				m_windoObj.SetActive(false);
			}
		}
	}
}
