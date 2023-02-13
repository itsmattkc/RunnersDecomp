using SaveData;
using Text;
using UnityEngine;

public class OptionAcceptInvite : MonoBehaviour
{
	private const string ATTACH_ANTHOR_NAME = "UI Root (2D)/Camera/menu_Anim/OptionUI/Anchor_5_MC";

	private SettingPartsAcceptInvite m_acceptInvite;

	private EasySnsFeed m_easySnsFeed;

	private bool m_loginFlag;

	private bool m_acceptedFlag;

	private ui_option_scroll m_ui_option_scroll;

	public void Setup(ui_option_scroll scroll)
	{
		if (scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		if (HudMenuUtility.IsSystemDataFlagStatus(SystemData.FlagStatus.FRIEDN_ACCEPT_INVITE))
		{
			m_acceptedFlag = true;
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "AcceptedInvite";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("Option", "accepted_invite_caption");
			info.message = TextUtility.GetCommonText("Option", "accepted_invite_text");
			GeneralWindow.Create(info);
		}
		else
		{
			m_loginFlag = false;
			SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface != null)
			{
				m_loginFlag = socialInterface.IsLoggedIn;
			}
			if (m_loginFlag)
			{
				PlayAcceptInvite();
			}
			else
			{
				m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/Anchor_5_MC");
			}
		}
		base.enabled = true;
	}

	private void SetAcceptInvite()
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			m_loginFlag = socialInterface.IsLoggedIn;
		}
		if (m_loginFlag)
		{
			PlayAcceptInvite();
		}
	}

	private void PlayAcceptInvite()
	{
		m_acceptInvite = base.gameObject.GetComponent<SettingPartsAcceptInvite>();
		if (m_acceptInvite == null)
		{
			m_acceptInvite = base.gameObject.AddComponent<SettingPartsAcceptInvite>();
			m_acceptInvite.Setup("UI Root (2D)/Camera/menu_Anim/OptionUI/Anchor_5_MC");
			m_acceptInvite.PlayStart();
		}
		else
		{
			m_acceptInvite.PlayStart();
		}
	}

	public void Update()
	{
		if (m_acceptedFlag)
		{
			if (GeneralWindow.IsCreated("AcceptedInvite") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				if (m_ui_option_scroll != null)
				{
					m_ui_option_scroll.OnEndChildPage();
				}
				base.enabled = false;
			}
		}
		else if (m_loginFlag)
		{
			if (m_acceptInvite != null && m_acceptInvite.IsEndPlay())
			{
				if (m_ui_option_scroll != null)
				{
					m_ui_option_scroll.OnEndChildPage();
				}
				base.enabled = false;
			}
		}
		else
		{
			if (m_easySnsFeed == null)
			{
				return;
			}
			switch (m_easySnsFeed.Update())
			{
			case EasySnsFeed.Result.COMPLETED:
				SetAcceptInvite();
				m_easySnsFeed = null;
				if (!m_loginFlag)
				{
					if (m_ui_option_scroll != null)
					{
						m_ui_option_scroll.OnEndChildPage();
					}
					base.enabled = false;
				}
				break;
			case EasySnsFeed.Result.FAILED:
				m_easySnsFeed = null;
				if (m_ui_option_scroll != null)
				{
					m_ui_option_scroll.OnEndChildPage();
				}
				base.enabled = false;
				break;
			}
		}
	}
}
