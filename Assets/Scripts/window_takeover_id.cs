using Message;
using Text;
using UnityEngine;

public class window_takeover_id : WindowBase
{
	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private UILabel m_headerTextLabel;

	[SerializeField]
	private UILabel m_passwordTextLabel;

	[SerializeField]
	private UILabel m_passwordLabel;

	private bool m_isEnd;

	private UIPlayAnimation m_uiAnimation;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
		OptionMenuUtility.TranObj(base.gameObject);
		base.enabled = false;
		if (m_closeBtn != null)
		{
			UIPlayAnimation component = m_closeBtn.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				EventDelegate.Add(component.onFinished, OnFinishedAnimationCallback, false);
			}
			UIButtonMessage uIButtonMessage = m_closeBtn.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = m_closeBtn.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.enabled = true;
				uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickCloseButton";
			}
		}
		TextUtility.SetCommonText(m_headerTextLabel, "Option", "take_over");
		TextUtility.SetCommonText(m_passwordTextLabel, "Option", "password");
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component2 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component2;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		if (m_passwordLabel != null)
		{
			m_passwordLabel.text = string.Empty;
		}
		SoundManager.SePlay("sys_window_open");
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
	}

	private void RequestServerGetMigrationPassword()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			if (string.IsNullOrEmpty(ServerInterface.MigrationPassword))
			{
				loggedInServerInterface.RequestServerGetMigrationPassword(null, base.gameObject);
			}
			else if (m_passwordLabel != null)
			{
				m_passwordLabel.text = ServerInterface.MigrationPassword;
			}
		}
	}

	public void PlayOpenWindow()
	{
		RequestServerGetMigrationPassword();
		m_isEnd = false;
		if (m_uiAnimation != null)
		{
			m_uiAnimation.Play(true);
		}
	}

	private void ServerGetMigrationPassword_Succeeded(MsgGetMigrationPasswordSucceed msg)
	{
		if (msg != null && m_passwordLabel != null)
		{
			m_passwordLabel.text = msg.m_migrationPassword;
		}
	}

	private void ServerGetMigrationPassword_Failed()
	{
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
		if (component != null)
		{
			component.SendMessage("OnClick");
		}
	}
}
