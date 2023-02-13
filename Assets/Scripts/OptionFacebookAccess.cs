using Text;
using UnityEngine;

public class OptionFacebookAccess : MonoBehaviour
{
	private enum State
	{
		INIT,
		IDLE,
		LOGIN,
		LOGOUT,
		LOGOUT_COMPLETE_SETTING,
		LOGOUT_COMPLETE,
		CLOSE
	}

	private window_event_setting m_eventSetting;

	private GameObject m_gameObject;

	private ui_option_scroll m_ui_option_scroll;

	private bool m_initFlag;

	private EasySnsFeed m_easySnsFeed;

	private State m_State;

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
			if (m_eventSetting != null)
			{
				m_eventSetting.Setup(window_event_setting.TextType.FACEBOOK_ACCESS);
				m_eventSetting.PlayOpenWindow();
				m_State = State.IDLE;
			}
		}
		else
		{
			m_initFlag = false;
			m_gameObject = HudMenuUtility.GetLoadMenuChildObject("window_event_setting", true);
		}
	}

	private void SetEventSetting()
	{
		if (m_gameObject != null && m_eventSetting == null)
		{
			m_eventSetting = m_gameObject.GetComponent<window_event_setting>();
		}
	}

	public void Update()
	{
		if (!m_initFlag)
		{
			m_initFlag = true;
			SetEventSetting();
			if (m_eventSetting != null)
			{
				m_eventSetting.Setup(window_event_setting.TextType.FACEBOOK_ACCESS);
				m_eventSetting.PlayOpenWindow();
				m_State = State.IDLE;
			}
			return;
		}
		switch (m_State)
		{
		case State.INIT:
			break;
		case State.IDLE:
			if (m_eventSetting != null && m_eventSetting.IsEnd)
			{
				switch (m_eventSetting.EndState)
				{
				case window_event_setting.State.CLOSE:
					CloseFunction();
					break;
				case window_event_setting.State.PRESS_LOGIN:
					m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/Anchor_5_MC");
					m_State = State.LOGIN;
					break;
				case window_event_setting.State.PRESS_LOGOUT:
					CreateLogoutWindow();
					m_State = State.LOGOUT;
					break;
				}
			}
			break;
		case State.LOGIN:
			if (m_easySnsFeed != null)
			{
				switch (m_easySnsFeed.Update())
				{
				case EasySnsFeed.Result.COMPLETED:
					m_easySnsFeed = null;
					CloseFunction();
					break;
				case EasySnsFeed.Result.FAILED:
					m_easySnsFeed = null;
					CloseFunction();
					break;
				}
			}
			break;
		case State.LOGOUT:
			if (!GeneralWindow.IsCreated("FacebookLogout") || !GeneralWindow.IsButtonPressed)
			{
				break;
			}
			if (GeneralWindow.IsYesButtonPressed)
			{
				SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
				if (socialInterface != null)
				{
					socialInterface.Logout();
					socialInterface.IsLoggedIn = false;
					PlayerImageManager instance = PlayerImageManager.Instance;
					if (instance != null)
					{
						instance.ClearAllPlayerImage();
					}
					HudMenuUtility.SetUpdateRankingFlag();
				}
				m_State = State.LOGOUT_COMPLETE_SETTING;
			}
			else
			{
				CloseFunction();
			}
			GeneralWindow.Close();
			break;
		case State.LOGOUT_COMPLETE_SETTING:
			CreateLogoutCompleteWindow();
			m_State = State.LOGOUT_COMPLETE;
			break;
		case State.LOGOUT_COMPLETE:
			if (GeneralWindow.IsCreated("LogoutComplete") && GeneralWindow.IsButtonPressed)
			{
				CloseFunction();
				GeneralWindow.Close();
			}
			break;
		}
	}

	private void CreateLogoutWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "FacebookLogout";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		info.caption = TextUtility.GetCommonText("Option", "logout");
		info.message = TextUtility.GetCommonText("Option", "logout_message");
		GeneralWindow.Create(info);
	}

	private void CreateLogoutCompleteWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "LogoutComplete";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetCommonText("Option", "logout");
		info.message = TextUtility.GetCommonText("Option", "logout_complete");
		GeneralWindow.Create(info);
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
