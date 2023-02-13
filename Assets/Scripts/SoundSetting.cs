using UnityEngine;

public class SoundSetting : MonoBehaviour
{
	private window_sound_setiing m_soundSetiing;

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
			if (m_soundSetiing != null)
			{
				m_soundSetiing.PlayOpenWindow();
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_sound_setiing", true);
		}
	}

	private void SetSoundSetting()
	{
		if (m_gameObject != null && m_soundSetiing == null)
		{
			m_soundSetiing = m_gameObject.GetComponent<window_sound_setiing>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetSoundSetting();
			if (m_soundSetiing != null)
			{
				m_soundSetiing.PlayOpenWindow();
			}
		}
		else
		{
			if (!(m_soundSetiing != null) || !m_soundSetiing.IsEnd)
			{
				return;
			}
			if (m_ui_option_scroll != null)
			{
				if (m_soundSetiing.IsOverwrite)
				{
					m_ui_option_scroll.SetSystemSaveFlag();
				}
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
