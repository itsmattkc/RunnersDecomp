using SaveData;
using UnityEngine;

public class OptionPushNotification : MonoBehaviour
{
	private const string ATTACH_ANTHOR_NAME = "UI Root (2D)/Camera/Anchor_5_MC";

	private SettingPartsPushNotice m_pushNotice;

	private ui_option_scroll m_ui_option_scroll;

	public void Setup(ui_option_scroll scroll)
	{
		base.enabled = true;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		if (m_pushNotice != null)
		{
			m_pushNotice.PlayStart();
			return;
		}
		m_pushNotice = base.gameObject.AddComponent<SettingPartsPushNotice>();
		if ((bool)m_pushNotice)
		{
			m_pushNotice.Setup("UI Root (2D)/Camera/Anchor_5_MC");
			m_pushNotice.PlayStart();
		}
	}

	public void Update()
	{
		if (!(m_pushNotice != null) || !m_pushNotice.IsEndPlay())
		{
			return;
		}
		if (m_ui_option_scroll != null)
		{
			if (m_pushNotice.IsOverwrite)
			{
				SystemSaveManager instance = SystemSaveManager.Instance;
				if (instance != null)
				{
					instance.SaveSystemData();
				}
				m_ui_option_scroll.ResetSystemSaveFlag();
			}
			m_ui_option_scroll.OnEndChildPage();
		}
		base.enabled = false;
		SetActivePushNoticeObject(false);
	}

	private void SetActivePushNoticeObject(bool flag)
	{
		if (m_pushNotice != null)
		{
			m_pushNotice.SetWindowActive(flag);
		}
	}
}
