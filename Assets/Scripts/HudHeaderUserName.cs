using UnityEngine;

public class HudHeaderUserName : MonoBehaviour
{
	private const string m_name_label_path = "Anchor_1_TL/mainmenu_info_user/Btn_honor/img_bg_name/Lbl_username";

	private UILabel m_ui_name_label;

	private bool m_initEnd;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		if (m_initEnd)
		{
			return;
		}
		GameObject mainMenuCmnUIObject = HudMenuUtility.GetMainMenuCmnUIObject();
		if (mainMenuCmnUIObject != null)
		{
			Transform transform = mainMenuCmnUIObject.transform.FindChild("Anchor_1_TL/mainmenu_info_user/Btn_honor/img_bg_name/Lbl_username");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject != null)
				{
					m_ui_name_label = gameObject.GetComponent<UILabel>();
				}
			}
		}
		m_initEnd = true;
	}

	public void OnUpdateSaveDataDisplay()
	{
		Initialize();
		if (m_ui_name_label != null)
		{
			ServerSettingState settingState = ServerInterface.SettingState;
			if (settingState != null)
			{
				m_ui_name_label.text = settingState.m_userName;
			}
			else
			{
				m_ui_name_label.text = string.Empty;
			}
		}
	}
}
