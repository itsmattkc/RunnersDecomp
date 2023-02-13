using UnityEngine;

public class OptionUserName : MonoBehaviour
{
	private const string ATTACH_ANTHOR_NAME = "UI Root (2D)/Camera/Anchor_5_MC";

	private SettingUserName m_settingName;

	private ui_option_scroll m_ui_option_scroll;

	public void Setup(ui_option_scroll scroll)
	{
		if (scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		if (m_settingName == null)
		{
			GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
			if (menuAnimUIObject != null)
			{
				m_settingName = GameObjectUtil.FindChildGameObjectComponent<SettingUserName>(menuAnimUIObject, "window_name_setting");
			}
		}
		if (m_settingName != null)
		{
			m_settingName.SetCancelButtonUseFlag(true);
			m_settingName.Setup("UI Root (2D)/Camera/Anchor_5_MC");
			m_settingName.PlayStart();
		}
		base.enabled = true;
	}

	public void Update()
	{
		if (m_settingName != null && m_settingName.IsEndPlay())
		{
			if (m_ui_option_scroll != null)
			{
				m_ui_option_scroll.OnEndChildPage();
			}
			base.enabled = false;
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		}
	}
}
