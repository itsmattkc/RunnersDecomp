using SaveData;
using Text;
using UnityEngine;

public class CheckID : MonoBehaviour
{
	private enum State
	{
		INIT,
		FIRST_CAUTION,
		CHECK_PASSWORD,
		INPUT_USERPASS,
		GET_MOVING_PASSWORD,
		IDLE,
		CLOSE
	}

	private const string ATTACH_ANTHOR_NAME = "UI Root (2D)/Camera/Anchor_5_MC";

	private window_id_info m_idInfo;

	private GameObject m_gameObject;

	private ui_option_scroll m_ui_option_scroll;

	private SettingTakeoverPassword m_settingPassword;

	private bool m_initFlag;

	private State m_State;

	public void Setup(ui_option_scroll scroll)
	{
		base.enabled = true;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		if (m_gameObject == null)
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_id_info", true);
		}
		m_State = State.INIT;
	}

	private void SetIdInfo()
	{
		if (m_gameObject != null && m_idInfo == null)
		{
			m_idInfo = m_gameObject.GetComponent<window_id_info>();
		}
	}

	public void Update()
	{
		switch (m_State)
		{
		case State.GET_MOVING_PASSWORD:
			break;
		case State.CLOSE:
			break;
		case State.INIT:
			CreateFirstCautionWindow();
			break;
		case State.FIRST_CAUTION:
			if (GeneralWindow.IsCreated("FirstCaution") && GeneralWindow.IsButtonPressed)
			{
				if (m_gameObject != null)
				{
					m_gameObject.SetActive(false);
				}
				GeneralWindow.Close();
				m_State = State.CHECK_PASSWORD;
			}
			break;
		case State.CHECK_PASSWORD:
		{
			string takeoverID = SystemSaveManager.GetTakeoverID();
			if (string.IsNullOrEmpty(takeoverID))
			{
				CreateUserPassWindow();
			}
			else
			{
				SetupInfoWindow();
			}
			break;
		}
		case State.INPUT_USERPASS:
			if (m_settingPassword != null && m_settingPassword.IsEndPlay())
			{
				if (m_settingPassword.isCancel)
				{
					CloseFunction();
				}
				else
				{
					SetupInfoWindow();
				}
			}
			break;
		case State.IDLE:
			if (!(m_idInfo != null) || !m_idInfo.IsEnd)
			{
				break;
			}
			if (m_idInfo.IsPassResetEnd)
			{
				if (m_gameObject != null)
				{
					m_gameObject.SetActive(false);
				}
				CreateUserPassWindow();
			}
			else
			{
				CloseFunction();
			}
			break;
		}
	}

	private void CreateFirstCautionWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "FirstCaution";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetCommonText("Option", "take_over_attention");
		info.message = TextUtility.GetCommonText("Option", "take_over_attention_text");
		GeneralWindow.Create(info);
		m_State = State.FIRST_CAUTION;
	}

	private void CreateUserPassWindow()
	{
		if (m_settingPassword == null)
		{
			m_settingPassword = base.gameObject.AddComponent<SettingTakeoverPassword>();
		}
		if (m_settingPassword != null)
		{
			m_settingPassword.SetCancelButtonUseFlag(true);
			m_settingPassword.Setup("UI Root (2D)/Camera/Anchor_5_MC");
			m_settingPassword.PlayStart();
		}
		m_State = State.INPUT_USERPASS;
	}

	private void SetupInfoWindow()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetIdInfo();
		}
		m_gameObject.SetActive(true);
		if (m_idInfo != null)
		{
			m_idInfo.PlayOpenWindow();
		}
		m_State = State.IDLE;
	}

	private void CloseFunction()
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
		m_State = State.CLOSE;
	}
}
